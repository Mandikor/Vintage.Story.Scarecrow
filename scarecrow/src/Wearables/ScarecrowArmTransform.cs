using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>One shared arm adjustment, mirrored for the right arm.</summary>
    public static class ScarecrowArmTransform
    {
        public const string SharedAttribute = "onScarecrowTransform";
        public const string NeckAttribute = "onScarecrowNeckTransform";
        private sealed class Entry
        {
            public readonly JToken Token;
            public readonly bool Mirrored;
            public readonly ModelTransform Transform;
            public readonly float[] Matrix;
            public Entry(JToken token, ModelTransform transform = null, float[] matrix = null, bool mirrored = false)
            { Token = token; Transform = transform; Matrix = matrix; Mirrored = mirrored; }
        }
        private sealed class Cache { public Entry Left = new(null), Right = new(null); }
        private static readonly ConditionalWeakTable<CollectibleObject, Cache> cache = new();

        public static string AttributeForSlot(string slot) => slot switch
        {
            "sc-leftarm" or "sc-rightarm" => SharedAttribute,
            _ => null
        };

        // The editor replaces the attribute token. Cache immutable parsed values so animated
        // attachments do not deserialize JSON or allocate matrices on every render pass.
        private static Entry Read(ItemStack stack, string slot)
        {
            string attribute = AttributeForSlot(slot);
            if (attribute == null || stack?.Collectible == null) return null;
            Cache values = cache.GetOrCreateValue(stack.Collectible);
            Entry entry = slot == "sc-leftarm"
                ? System.Threading.Volatile.Read(ref values.Left)
                : System.Threading.Volatile.Read(ref values.Right);
            JToken token = stack.ItemAttributes?[attribute].Token;
            bool mirrored = token != null && slot == "sc-rightarm";
            if (ReferenceEquals(token, entry.Token) && mirrored == entry.Mirrored) return entry;
            Entry updated = new(token, mirrored: mirrored);
            if (token is JObject)
            {
                try
                {
                    ModelTransform transform = token.ToObject<ModelTransform>().EnsureDefaultValues();
                    // Conjugate the adjustment by reflection at local x = 0.5:
                    // H * T * H. This mirrors placement, not the mesh itself, so
                    // winding, textures and positive model scale remain intact.
                    if (mirrored)
                    {
                        transform.Translation.X = -transform.Translation.X;
                        transform.Rotation.Y = -transform.Rotation.Y;
                        transform.Rotation.Z = -transform.Rotation.Z;
                        transform.Origin.X = 1 - transform.Origin.X;
                    }
                    float[] matrix = transform.AsMatrix;
                    if (matrix.All(float.IsFinite)) updated = new Entry(token, transform, matrix, mirrored);
                }
                catch (JsonException) { /* Invalid optional metadata must not break entity rendering. */ }
            }
            // Publish complete immutable entries: tessellation and the editor can run on
            // different threads, so readers must never observe a partially rebuilt cache.
            if (slot == "sc-leftarm") System.Threading.Volatile.Write(ref values.Left, updated);
            else System.Threading.Volatile.Write(ref values.Right, updated);
            return updated;
        }

        internal static float[] GetMatrix(ItemStack stack, string slot) => Read(stack, slot)?.Matrix;

        // Neck attachments use their own static frame, independent of arm mirroring.
        // Read only during tessellation; never move the shared source model or neck slot.
        internal static ModelTransform GetNeckTransform(ItemStack stack)
        {
            if (stack?.ItemAttributes?[NeckAttribute].Token is not JObject token) return null;
            try
            {
                ModelTransform transform = token.ToObject<ModelTransform>().EnsureDefaultValues();
                return transform.AsMatrix.All(float.IsFinite) ? transform : null;
            }
            catch (JsonException) { return null; }
        }

        // A mirrored adjustment alone does not reverse an asymmetric model.
        // Reflect only the private right-tool mesh; native MatrixTransform also
        // transforms packed normals and face directions. Reflection reverses
        // triangle handedness, so restore outward winding for culling/shadows.
        internal static void MirrorToolMesh(MeshData mesh)
        {
            int[] normals = mesh.Normals == null ? null : (int[])mesh.Normals.Clone();
            mesh.MatrixTransform(new Matrixf().Translate(1, 0, 0).Scale(-1, 1, 1).Values);
            // Preserve Y/Z/W in the optional packed 10-bit normal buffer. The
            // engine's legacy unpacker loses negative component signs there.
            if (normals != null)
                for (int i = 0; i < mesh.VerticesCount; i++)
                {
                    int x = normals[i] & 1023;
                    mesh.Normals[i] = (normals[i] & ~1023) | (x is 0 or 1023 ? 0 : x ^ 1023);
                }
            for (int i = 0; i + 2 < mesh.IndicesCount; i += 3)
                (mesh.Indices[i + 1], mesh.Indices[i + 2]) = (mesh.Indices[i + 2], mesh.Indices[i + 1]);
        }

        // Snapshot the already cloned entity shape before the engine attaches this slot's
        // geometry. Only newly inserted roots are wrapped; the body and other gear are untouched.
        internal static HashSet<ShapeElement> Snapshot(Shape shape)
        {
            var result = new HashSet<ShapeElement>();
            void Visit(ShapeElement[] elements)
            {
                if (elements == null) return;
                foreach (ShapeElement element in elements) { result.Add(element); Visit(element.Children); }
            }
            Visit(shape.Elements);
            return result;
        }

        internal static void ApplyToAdded(Shape shape, HashSet<ShapeElement> previous, ItemStack stack, string slot)
            => ApplyToAdded(shape, previous, Read(stack, slot)?.Transform);

        internal static void ApplyToAdded(Shape shape, HashSet<ShapeElement> previous, ModelTransform transform)
        {
            if (transform == null) return;
            void Visit(ShapeElement[] elements, ShapeElement parent)
            {
                if (elements == null) return;
                for (int i = 0; i < elements.Length; i++)
                {
                    if (previous.Contains(elements[i])) Visit(elements[i].Children, elements[i]);
                    else elements[i] = Wrap(elements[i], parent, transform);
                }
            }
            Visit(shape.Elements, null);
        }

        // Separate translation, single-axis rotations and scale preserve ModelTransform's
        // T * O * Rx * Ry * Rz * S * -O order in both engine animation transform versions.
        internal static ShapeElement Wrap(ShapeElement child, ShapeElement parent, ModelTransform tf)
        {
            ShapeElement Node(string suffix, double x = 0, double y = 0, double z = 0) => new()
            {
                Name = child.Name + "-scarecrow-transform-" + suffix,
                From = new[] { x, y, z }, To = new[] { x, y, z },
                RotationOrigin = new double[3], JointId = child.JointId
            };
            ShapeElement outer = Node("translate", 16 * (tf.Translation.X + tf.Origin.X),
                16 * (tf.Translation.Y + tf.Origin.Y), 16 * (tf.Translation.Z + tf.Origin.Z));
            ShapeElement xrot = Node("x"), yrot = Node("y"), zrot = Node("z"), scale = Node("scale"),
                offset = Node("origin", -16 * tf.Origin.X, -16 * tf.Origin.Y, -16 * tf.Origin.Z);
            xrot.RotationX = tf.Rotation.X; yrot.RotationY = tf.Rotation.Y; zrot.RotationZ = tf.Rotation.Z;
            scale.ScaleX = tf.ScaleXYZ.X; scale.ScaleY = tf.ScaleXYZ.Y; scale.ScaleZ = tf.ScaleXYZ.Z;
            ShapeElement[] chain = { outer, xrot, yrot, zrot, scale, offset, child };
            outer.ParentElement = parent;
            for (int i = 0; i < chain.Length - 1; i++)
            {
                chain[i].Children = new[] { chain[i + 1] };
                chain[i + 1].ParentElement = chain[i];
            }
            return outer;
        }
    }
}

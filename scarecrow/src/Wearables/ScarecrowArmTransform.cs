using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Optional per-arm visual adjustments, independent of attachment permission and item categories.</summary>
    public static class ScarecrowArmTransform
    {
        public const string LeftAttribute = "onScarecrowLeftArmTransform";
        public const string RightAttribute = "onScarecrowRightArmTransform";
        private sealed class Entry
        {
            public readonly JToken Token;
            public readonly ModelTransform Transform;
            public readonly float[] Matrix;
            public Entry(JToken token, ModelTransform transform = null, float[] matrix = null)
            { Token = token; Transform = transform; Matrix = matrix; }
        }
        private sealed class Cache { public Entry Left = new(null), Right = new(null); }
        private static readonly ConditionalWeakTable<CollectibleObject, Cache> cache = new();

        public static string AttributeForSlot(string slot) => slot switch
        {
            "sc-leftarm" => LeftAttribute,
            "sc-rightarm" => RightAttribute,
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
            if (ReferenceEquals(token, entry.Token)) return entry;
            Entry updated = new(token);
            if (token is JObject)
            {
                try
                {
                    ModelTransform transform = token.ToObject<ModelTransform>().EnsureDefaultValues();
                    float[] matrix = transform.AsMatrix;
                    if (matrix.All(float.IsFinite)) updated = new Entry(token, transform, matrix);
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
        {
            ModelTransform transform = Read(stack, slot)?.Transform;
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

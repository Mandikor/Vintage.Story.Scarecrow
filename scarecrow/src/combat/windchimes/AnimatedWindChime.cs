using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>
    /// Splits an authored block shape into a stationary support and rigid hanging subtrees.
    /// Meshes are uploaded once; only their transforms change while rendering. No shared asset is edited.
    /// </summary>
    internal sealed class AnimatedWindChime : IDisposable
    {
        internal sealed class Part
        {
            public string Name;
            public ShapeElement Element;
            public float[] Parent, InverseRest;
            public MultiTextureMeshRef Mesh;
            public readonly ElementPose Pose = new();
            private readonly float[] local = Mat4f.Create(), animated = Mat4f.Create();
            public readonly float[] Transform = Mat4f.Create();

            public void SetAngles(float pitch, float yaw, float roll)
            {
                Pose.degOffX = pitch; Pose.degOffY = yaw; Pose.degOffZ = roll;
                Mat4f.Identity(local);
                Element.GetLocalTransformMatrix(0, local, Pose);
                Mat4f.Mul(animated, Parent, local);
                Mat4f.Mul(Transform, animated, InverseRest);
            }
        }

        public MultiTextureMeshRef Support { get; private set; }
        public Part[] Parts { get; private set; } = Array.Empty<Part>();
        private readonly WindChimeMotion motion;

        private AnimatedWindChime(long entityId, int arm) => motion = new WindChimeMotion(entityId, arm);

        // Exposed internally for asset-based checks: identify every chime, including fountain chimes 6–9.
        internal static Part[] FindParts(Shape shape)
        {
            var result = new List<Part>();
            void Walk(ShapeElement[] elements, float[] parent)
            {
                if (elements == null) return;
                foreach (ShapeElement element in elements)
                {
                    float[] rest = Mat4f.Mul(new float[16], parent, element.GetLocalTransformMatrix(0));
                    if (element.Name != null && element.Name.StartsWith("chime", StringComparison.OrdinalIgnoreCase)
                        && int.TryParse(element.Name.AsSpan(5), out int number) && number > 0)
                    {
                        result.Add(new Part { Name = element.Name, Element = element, Parent = parent,
                            InverseRest = Mat4f.Invert(new float[16], rest) });
                        // Children move rigidly with this hanging piece, not as duplicate animated parts.
                    }
                    else Walk(element.Children, rest);
                }
            }
            Walk(shape.Elements, Mat4f.Create());
            return result.ToArray();
        }

        internal static Shape SelectGeometry(JObject original, string selected, ISet<string> moving)
        {
            var copy = (JObject)original.DeepClone();
            void Walk(JArray elements, string owner)
            {
                if (elements == null) return;
                foreach (JObject element in elements)
                {
                    string name = element["name"]?.ToString();
                    string group = moving.Contains(name ?? "") ? name : owner;
                    if (element["faces"] is JObject faces)
                        foreach (JObject face in faces.Properties().Select(p => p.Value).OfType<JObject>())
                        {
                            if (group != selected) face["enabled"] = false;
                            // New rigid-body motion replaces shader wind, never layers over it.
                            face.Remove("windMode"); face.Remove("windData");
                        }
                    Walk(element["children"] as JArray, group);
                }
            }
            Walk(copy["elements"] as JArray, null);
            return copy.ToObject<Shape>();
        }

        public static AnimatedWindChime TryCreate(ICoreClientAPI api, Block block, long entityId, int arm)
        {
            AssetLocation location = block.Shape?.Base?.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            JObject source = location == null ? null : api.Assets.TryGet(location)?.ToObject<JObject>();
            if (source == null) return null;
            Part[] parts = FindParts(source.ToObject<Shape>());
            if (parts.Length == 0) return null;
            var result = new AnimatedWindChime(entityId, arm) { Parts = parts };
            try
            {
                var moving = new HashSet<string>(parts.Select(p => p.Name));
                MultiTextureMeshRef Upload(string selected)
                {
                    api.Tesselator.TesselateShape(block, SelectGeometry(source, selected, moving), out MeshData mesh);
                    return mesh.VerticesCount == 0 ? null : api.Render.UploadMultiTextureMesh(mesh);
                }
                result.Support = Upload(null);
                foreach (Part part in parts) part.Mesh = Upload(part.Name);
                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        public void Advance(float dt, double windX, double windZ)
        {
            motion.Advance(dt, windX, windZ);
            for (int i = 0; i < Parts.Length; i++)
            {
                var angles = motion.GetAngles(i);
                Parts[i].SetAngles(angles.pitch, angles.yaw, angles.roll);
            }
        }

        public void Dispose()
        {
            Support?.Dispose(); Support = null;
            foreach (Part part in Parts) { part.Mesh?.Dispose(); part.Mesh = null; }
        }
    }
}

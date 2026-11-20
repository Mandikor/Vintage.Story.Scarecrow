using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>A private ceiling-model mesh swinging about its arm suspension point, never a placed block entity.</summary>
    internal sealed class AnimatedArmLantern : IDisposable
    {
        public MultiTextureMeshRef Mesh { get; private set; }
        public readonly Matrixf Transform = new();
        private readonly WindChimeMotion motion;
        private readonly float scale;

        public static bool Matches(ItemStack stack) => stack?.Block != null
            && stack.ItemAttributes?["scarecrowArmLantern"].AsBool(false) == true;

        public AnimatedArmLantern(ICoreClientAPI api, ItemStack stack, string slot, long entityId, int arm)
        {
            CompositeShape model = IAttachableToEntity.FromCollectible(stack.Block)?.GetAttachedShape(stack, slot);
            Shape shape = model?.Base == null ? null : Shape.TryGet(api, model.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json"));
            if (shape == null) throw new InvalidOperationException("Missing arm lantern shape: " + model?.Base);
            MeshData mesh;
            if (stack.Block is BlockLantern vanilla)
            {
                // Vanilla stores metal, lining and glass on the stack, not in the block code.
                // Use its own texture supplier so differently equipped arms retain their materials.
                mesh = vanilla.GenMesh(api, stack.Attributes.GetString("material", "copper"),
                    stack.Attributes.GetString("lining", "plain"), stack.Attributes.GetString("glass", "quartz"), shape);
            }
            else api.Tesselator.TesselateShape(stack.Block, shape, out mesh,
                new Vec3f(model.rotateX, model.rotateY, model.rotateZ));
            Mesh = api.Render.UploadMultiTextureMesh(mesh);
            scale = stack.ItemAttributes["scarecrowArmLanternScale"].AsFloat(.6f);
            motion = new WindChimeMotion(entityId, arm);
            UpdateTransform(0, 0, 0);
        }

        public void Advance(float dt, double windX, double windZ)
        {
            motion.Advance(dt, windX, windZ);
            var angles = motion.GetAngles(0);
            UpdateTransform(angles.pitch * .65f, angles.yaw * .3f, angles.roll * .65f);
        }

        private void UpdateTransform(float pitch, float yaw, float roll)
        {
            // Ceiling models share the block's top-center hanging point. Scale and rotate around it.
            Transform.Identity().Translate(.5f, 1, .5f)
                .RotateX(pitch * GameMath.DEG2RAD).RotateY(yaw * GameMath.DEG2RAD)
                .RotateZ(roll * GameMath.DEG2RAD).Scale(scale, scale, scale).Translate(-.5f, -1, -.5f);
        }

        public void Dispose() { Mesh?.Dispose(); Mesh = null; }
    }
}

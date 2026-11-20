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
        private readonly CenserArmSmoke smoke;
        private readonly ArmAttachmentParticles oilLampParticles;

        public static bool Matches(ItemStack stack) => stack?.Block != null
            && (stack.ItemAttributes?["scarecrowArmLantern"].AsBool(false) == true
                || stack.ItemAttributes?["scarecrowHangingDecoration"].AsBool(false) == true);

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
            else
            {
                Block textureBlock = stack.Block;
                if (stack.ItemAttributes["scarecrowUseCeilingTextures"].AsBool(false))
                {
                    // Hanging shapes can contain texture keys absent from the inventory/ground
                    // atlas mapping (oil-lamp rope, More Lanterns ropes/paper/metal). Use the
                    // actual ceiling variant's texture source, retaining state/color/brightness.
                    AssetLocation code = stack.Block.Code;
                    textureBlock = api.World.GetBlock(new AssetLocation(code.Domain, code.Path[..^2] + "down")) ?? textureBlock;
                }
                api.Tesselator.TesselateShape(textureBlock, shape, out mesh,
                    new Vec3f(model.rotateX, model.rotateY, model.rotateZ));
            }
            // Small ground models can align their top with the shared ceiling pivot through
            // attachment-only metadata. Move this private mesh; never alter the placed shape.
            float[] offset = stack.ItemAttributes["scarecrowArmLanternOffset"].AsArray<float>();
            if (offset?.Length == 3) mesh.Translate(offset[0], offset[1], offset[2]);
            Mesh = api.Render.UploadMultiTextureMesh(mesh);
            scale = stack.ItemAttributes["scarecrowArmLanternScale"].AsFloat(.6f);
            motion = new WindChimeMotion(entityId, arm);
            if (stack.ItemAttributes["scarecrowCenserSmoke"].AsBool(false) && api.ModLoader.IsModEnabled("censership"))
                smoke = new CenserArmSmoke(api, stack.Block, scale);
            if (stack.ItemAttributes["scarecrowOilLampParticles"].AsBool(false))
            {
                // The placed-block emitter uses a fixed height that does not match the rendered
                // hanging shape. Locate the model's flame pivot before applying the arm transform.
                oilLampParticles = new ArmAttachmentParticles(stack.Block.ParticleProperties,
                    FindOilLampWick(shape), scale);
            }
            UpdateTransform(0, 0, 0);
        }

        public void Advance(float dt, double windX, double windZ)
        {
            motion.Advance(dt, windX, windZ);
            var angles = motion.GetAngles(0);
            UpdateTransform(angles.pitch * .65f, angles.yaw * .3f, angles.roll * .65f);
        }

        internal static Vec3f FindOilLampWick(Shape shape)
        {
            Vec3f Find(ShapeElement[] elements, float[] parent)
            {
                if (elements == null) return null;
                foreach (ShapeElement element in elements)
                {
                    if (string.Equals(element.Name, "Flame", StringComparison.OrdinalIgnoreCase))
                    {
                        double[] origin = element.RotationOrigin ?? element.From;
                        float[] point = Mat4f.MulWithVec4(parent, (float)origin[0] / 16,
                            (float)origin[1] / 16, (float)origin[2] / 16, 1);
                        return new Vec3f(point[0], point[1], point[2]);
                    }
                    float[] matrix = Mat4f.Mul(new float[16], parent, element.GetLocalTransformMatrix(0));
                    Vec3f found = Find(element.Children, matrix);
                    if (found != null) return found;
                }
                return null;
            }
            return Find(shape.Elements, Mat4f.Create()) ?? new Vec3f(.5f, 7f / 32, .5f);
        }

        private void UpdateTransform(float pitch, float yaw, float roll)
        {
            // Ceiling models share the block's top-center hanging point. Scale and rotate around it.
            Transform.Identity().Translate(.5f, 1, .5f)
                .RotateX(pitch * GameMath.DEG2RAD).RotateY(yaw * GameMath.DEG2RAD)
                .RotateZ(roll * GameMath.DEG2RAD).Scale(scale, scale, scale).Translate(-.5f, -1, -.5f);
        }

        public void Dispose() { Mesh?.Dispose(); Mesh = null; }

        public void EmitParticles(ICoreClientAPI api, float dt, Matrixf cameraRelativeModel)
        {
            smoke?.Emit(api, dt, cameraRelativeModel);
            oilLampParticles?.Emit(api, dt, cameraRelativeModel);
        }
    }
}

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Animates attached hanging pieces independently of placed-block animation mods.</summary>
    public class WindChimeScarecrowRenderer : EntityShapeRenderer
    {
        private readonly Block[] blocks = new Block[2];
        private readonly ItemStack[] renderedStacks = new ItemStack[2];
        private readonly MultiTextureMeshRef[] meshes = new MultiTextureMeshRef[2];
        private readonly Matrixf chimeMatrix = new();
        private readonly Matrixf pieceMatrix = new();
        private readonly AnimatedWindChime[] animated = new AnimatedWindChime[2];
        private readonly AnimatedArmLantern[] lanterns = new AnimatedArmLantern[2];
        private long lastMotionUpdate = -1;

        public WindChimeScarecrowRenderer(Entity entity, ICoreClientAPI api) : base(entity, api) { }

        protected override void onMeshReady(MeshData meshData)
        {
            base.onMeshReady(meshData);
            ClearMeshes();
        }

        public override void DoRender3DOpaque(float dt, bool isShadowPass)
        {
            base.DoRender3DOpaque(dt, isShadowPass);
            if (isSpectator) return;
            RenderWindChimes(dt, isShadowPass);
        }

        // Reuse meshes per arm; apply the animated mount transform in both opaque and shadow passes.
        private void RenderWindChimes(float dt, bool isShadowPass)
        {
            if (!entity.Alive)
            {
                ClearMeshes();
                return;
            }
            InventoryBase inventory = entity.GetBehavior<ScarecrowAttachments>()?.Inventory;
            if (inventory == null) return;
            long now = entity.World.ElapsedMilliseconds;
            bool advance = !isShadowPass && now != lastMotionUpdate;
            if (advance) lastMotionUpdate = now;
            for (int index = 0; index < 2 && index < inventory.Count; index++)
            {
                ItemStack stack = inventory[index].Itemstack;
                bool lantern = AnimatedArmLantern.Matches(stack);
                Block block = lantern || stack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true ? stack.Block : null;
                if (blocks[index] != block || (lantern && !ReferenceEquals(renderedStacks[index], stack)))
                {
                    meshes[index]?.Dispose();
                    animated[index]?.Dispose();
                    lanterns[index]?.Dispose();
                    lanterns[index] = null;
                    animated[index] = null;
                    meshes[index] = null;
                    blocks[index] = block;
                    renderedStacks[index] = stack;
                    if (block != null)
                    {
                        // These are entity attachments, not placed block entities. Always use our
                        // private animation; Immersive Lanterns may animate placed copies separately.
                        try
                        {
                            if (lantern) lanterns[index] = new AnimatedArmLantern(capi, stack,
                                index == 0 ? "sc-leftarm" : "sc-rightarm", entity.EntityId, index);
                            else animated[index] = AnimatedWindChime.TryCreate(capi, block, entity.EntityId, index);
                        }
                        catch (System.Exception exception)
                        {
                            capi.Logger.Warning("[Scarecrow] Cannot animate attachment {0}; using static rendering: {1}", block.Code, exception.Message);
                        }
                        if (animated[index] == null && lanterns[index] == null)
                        {
                            capi.Tesselator.TesselateBlock(block, out MeshData mesh);
                            meshes[index] = capi.Render.UploadMultiTextureMesh(mesh);
                        }
                    }
                }
                if (meshes[index] == null && animated[index] == null && lanterns[index] == null) continue;
                AttachmentPointAndPose point = entity.AnimManager?.Animator?.GetAttachmentPointPose(
                    lantern ? (index == 0 ? "ScarecrowLanternLeftAP" : "ScarecrowLanternRightAP")
                        : (index == 0 ? "ScarecrowWindChimeLeftAP" : "ScarecrowWindChimeRightAP"));
                if (point == null) continue;
                chimeMatrix.Set(ModelMat).Mul(point.AnimModelMatrix);
                if (lanterns[index] is AnimatedArmLantern armLantern)
                {
                    if (advance)
                    {
                        Vec3d wind = entity.World.BlockAccessor.GetWindSpeedAt(entity.Pos.XYZ);
                        GetLocalWind(wind, chimeMatrix.Values, out double x, out double z);
                        armLantern.Advance(dt, x, z);
                    }
                    pieceMatrix.Set(chimeMatrix.Values).Mul(armLantern.Transform.Values);
                    RenderMesh(armLantern.Mesh, pieceMatrix, isShadowPass, true);
                }
                else if (animated[index] is AnimatedWindChime animation)
                {
                    // Block tessellation normally applies the composite shape rotation. Our split meshes
                    // are unrotated; apply that same rotation to their common frame, including shadows.
                    chimeMatrix.Translate(.5f, .5f, .5f)
                        .RotateX(block.Shape.rotateX * GameMath.DEG2RAD)
                        .RotateY(block.Shape.rotateY * GameMath.DEG2RAD)
                        .RotateZ(block.Shape.rotateZ * GameMath.DEG2RAD).Translate(-.5f, -.5f, -.5f);
                    if (advance)
                    {
                        Vec3d wind = entity.World.BlockAccessor.GetWindSpeedAt(entity.Pos.XYZ);
                        float[] m = chimeMatrix.Values;
                        double xLength = System.Math.Sqrt(m[0] * m[0] + m[2] * m[2]);
                        double zLength = System.Math.Sqrt(m[8] * m[8] + m[10] * m[10]);
                        animation.Advance(dt, (wind.X * m[0] + wind.Z * m[2]) / System.Math.Max(xLength, .0001),
                            (wind.X * m[8] + wind.Z * m[10]) / System.Math.Max(zLength, .0001));
                    }
                    RenderMesh(animation.Support, chimeMatrix, isShadowPass, true);
                    foreach (AnimatedWindChime.Part part in animation.Parts)
                    {
                        pieceMatrix.Set(chimeMatrix.Values).Mul(part.Transform);
                        RenderMesh(part.Mesh, pieceMatrix, isShadowPass, true);
                    }
                }
                else RenderMesh(meshes[index], chimeMatrix, isShadowPass, false);
            }
        }

        private static void GetLocalWind(Vec3d wind, float[] m, out double x, out double z)
        {
            double xLength = System.Math.Sqrt(m[0] * m[0] + m[2] * m[2]);
            double zLength = System.Math.Sqrt(m[8] * m[8] + m[10] * m[10]);
            x = (wind.X * m[0] + wind.Z * m[2]) / System.Math.Max(xLength, .0001);
            z = (wind.X * m[8] + wind.Z * m[10]) / System.Math.Max(zLength, .0001);
        }

        private void RenderMesh(MultiTextureMeshRef mesh, Matrixf matrix, bool isShadowPass, bool localAnimation)
        {
            if (mesh == null) return;
            if (isShadowPass)
            {
                IShaderProgram shader = capi.Render.CurrentActiveShader;
                if (shader == null) return;
                Matrixf mvp = new Matrixf(capi.Render.CurrentProjectionMatrix)
                    .Mul(capi.Render.CurrentModelviewMatrix).Mul(matrix.Values);
                shader.UniformMatrix("mvpMatrix", mvp.Values);
                shader.Uniform("origin", 0f, 0f, 0f);
                capi.Render.RenderMultiTextureMesh(mesh, "tex2d");
            }
            else
            {
                IStandardShaderProgram shader = capi.Render.PreparedStandardShader(
                    (int)entity.Pos.X, (int)entity.Pos.InternalY, (int)entity.Pos.Z, color);
                try
                {
                    shader.DontWarpVertices = localAnimation ? 1 : 0;
                    shader.AddRenderFlags = 0;
                    shader.ModelMatrix = matrix.Values;
                    shader.ViewMatrix = capi.Render.CameraMatrixOriginf;
                    shader.ProjectionMatrix = capi.Render.CurrentProjectionMatrix;
                    capi.Render.RenderMultiTextureMesh(mesh, "tex");
                }
                finally
                {
                    shader.Stop();
                }
            }
        }

        // Meshes belong to this renderer and must be released on retessellation and disposal.
        private void ClearMeshes()
        {
            for (int index = 0; index < meshes.Length; index++)
            {
                meshes[index]?.Dispose();
                animated[index]?.Dispose();
                lanterns[index]?.Dispose();
                lanterns[index] = null;
                animated[index] = null;
                meshes[index] = null;
                blocks[index] = null;
                renderedStacks[index] = null;
            }
        }

        public override void Dispose()
        {
            ClearMeshes();
            base.Dispose();
        }
    }
}

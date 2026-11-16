using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Draws wind chimes with the block shader, which preserves their authored wind modes.</summary>
    public class WindChimeScarecrowRenderer : EntityShapeRenderer
    {
        private readonly Block[] blocks = new Block[2];
        private readonly MultiTextureMeshRef[] meshes = new MultiTextureMeshRef[2];
        private readonly Matrixf chimeMatrix = new();

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
            RenderWindChimes(isShadowPass);
        }

        private void RenderWindChimes(bool isShadowPass)
        {
            InventoryBase inventory = entity.GetBehavior<ScarecrowAttachments>()?.Inventory;
            if (inventory == null) return;
            for (int index = 0; index < 2 && index < inventory.Count; index++)
            {
                ItemStack stack = inventory[index].Itemstack;
                Block block = stack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true ? stack.Block : null;
                if (blocks[index] != block)
                {
                    meshes[index]?.Dispose();
                    meshes[index] = null;
                    blocks[index] = block;
                    if (block != null)
                    {
                        capi.Tesselator.TesselateBlock(block, out MeshData mesh);
                        meshes[index] = capi.Render.UploadMultiTextureMesh(mesh);
                    }
                }
                if (meshes[index] == null) continue;
                AttachmentPointAndPose point = entity.AnimManager?.Animator?.GetAttachmentPointPose(
                    index == 0 ? "ScarecrowWindChimeLeftAP" : "ScarecrowWindChimeRightAP");
                if (point == null) continue;
                chimeMatrix.Set(ModelMat).Mul(point.AnimModelMatrix);
                if (isShadowPass)
                {
                    IShaderProgram shader = capi.Render.CurrentActiveShader;
                    if (shader == null) continue;
                    Matrixf mvp = new Matrixf(capi.Render.CurrentProjectionMatrix)
                        .Mul(capi.Render.CurrentModelviewMatrix).Mul(chimeMatrix.Values);
                    shader.UniformMatrix("mvpMatrix", mvp.Values);
                    shader.Uniform("origin", 0f, 0f, 0f);
                    capi.Render.RenderMultiTextureMesh(meshes[index], "tex2d");
                }
                else
                {
                    IStandardShaderProgram shader = capi.Render.PreparedStandardShader(
                        (int)entity.Pos.X, (int)entity.Pos.InternalY, (int)entity.Pos.Z, color);
                    try
                    {
                        shader.DontWarpVertices = 0;
                        shader.AddRenderFlags = 0;
                        shader.ModelMatrix = chimeMatrix.Values;
                        shader.ViewMatrix = capi.Render.CameraMatrixOriginf;
                        shader.ProjectionMatrix = capi.Render.CurrentProjectionMatrix;
                        capi.Render.RenderMultiTextureMesh(meshes[index], "tex");
                    }
                    finally
                    {
                        shader.Stop();
                    }
                }
            }
        }

        private void ClearMeshes()
        {
            for (int index = 0; index < meshes.Length; index++)
            {
                meshes[index]?.Dispose();
                meshes[index] = null;
                blocks[index] = null;
            }
        }

        public override void Dispose()
        {
            ClearMeshes();
            base.Dispose();
        }
    }
}

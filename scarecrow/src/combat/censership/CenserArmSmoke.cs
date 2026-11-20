using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Client-only smoke for hanging censers: private copies of the original ceiling block's particle profiles.</summary>
    internal sealed class CenserArmSmoke
    {
        private readonly ArmAttachmentParticles emitter;

        public CenserArmSmoke(ICoreClientAPI api, Block inventoryBlock, float scale)
        {
            // Inventory/up profiles have a different source height. Hanging geometry needs down profiles.
            AssetLocation code = inventoryBlock.Code;
            var hangingCode = new AssetLocation(code.Domain, code.Path[..^2] + "down");
            Block hanging = api.World.GetBlock(hangingCode) ?? inventoryBlock;
            emitter = new ArmAttachmentParticles(hanging.ParticleProperties,
                new Vec3f(hanging.TopMiddlePos.X, hanging.TopMiddlePos.Y, hanging.TopMiddlePos.Z), scale);
        }

        public void Emit(ICoreClientAPI api, float dt, Matrixf model)
            => emitter.Emit(api, dt, model);
    }
}

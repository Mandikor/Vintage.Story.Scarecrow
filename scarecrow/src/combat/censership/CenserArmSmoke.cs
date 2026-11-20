using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Client-only smoke for hanging censers: private copies of the original ceiling block's particle profiles.</summary>
    internal sealed class CenserArmSmoke
    {
        private readonly AdvancedParticleProperties[] particles;
        private readonly Vec3f[] origins;
        private float elapsed;

        public CenserArmSmoke(ICoreClientAPI api, Block inventoryBlock, float scale)
        {
            // Inventory/up profiles have a different source height. Hanging geometry needs down profiles.
            AssetLocation code = inventoryBlock.Code;
            var hangingCode = new AssetLocation(code.Domain, code.Path[..^2] + "down");
            Block hanging = api.World.GetBlock(hangingCode) ?? inventoryBlock;
            particles = (hanging.ParticleProperties ?? Array.Empty<AdvancedParticleProperties>()).Select(p => p.Clone()).ToArray();
            origins = new Vec3f[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                AdvancedParticleProperties p = particles[i];
                origins[i] = new Vec3f(hanging.TopMiddlePos.X + p.PosOffset[0].avg,
                    hanging.TopMiddlePos.Y + p.PosOffset[1].avg, hanging.TopMiddlePos.Z + p.PosOffset[2].avg);
                foreach (NatFloat offset in p.PosOffset) { offset.avg = 0; offset.var *= scale; }
                p.Size.avg *= scale; p.Size.var *= scale;
                p.WindAffectednesAtPos = 1;
            }
        }

        public void Emit(ICoreClientAPI api, float dt, Matrixf model)
        {
            if (api.IsGamePaused || !float.IsFinite(dt) || dt <= 0 || particles.Length == 0) return;
            elapsed += Math.Min(dt, .2f);
            if (elapsed < .165f) return;
            elapsed %= .165f;
            Vec3d camera = api.World.Player?.Entity?.CameraPos;
            if (camera == null) return;
            for (int i = 0; i < particles.Length; i++)
            {
                Vec3f origin = origins[i];
                float[] at = Mat4f.MulWithVec4(model.Values, origin.X, origin.Y, origin.Z, 1);
                // ModelMat is camera-relative, matching the game's own held-item particle rendering.
                if (at[0] * at[0] + at[1] * at[1] + at[2] * at[2] > 32 * 32) continue;
                AdvancedParticleProperties p = particles[i];
                p.basePos.Set(camera.X + at[0], camera.Y + at[1], camera.Z + at[2]);
                api.World.SpawnParticles(p);
            }
        }
    }
}

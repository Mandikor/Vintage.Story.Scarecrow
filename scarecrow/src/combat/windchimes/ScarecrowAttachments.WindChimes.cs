using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Scarecrow
{
    // Partial implementation keeps optional-mod hooks out of the shared inventory rules.
    // The CLR type and behavior code remain unchanged, preserving saved inventories.
    public partial class ScarecrowAttachments
    {
        private WindChimeAttachmentSounds windChimeSounds;
        private bool windChimesEnabled;
        private float soundUpdateElapsed;

        private void RefreshCompatibilitySounds() => windChimeSounds?.Update(inv);

        private void DisposeCompatibilitySounds() => windChimeSounds?.Dispose();

        // Cache optional-mod availability once; servers and absent mods skip all sound work.
        private void InitializeWindChimeSounds()
        {
            windChimeSounds?.Dispose();
            windChimeSounds = null;
            soundUpdateElapsed = 0;
            windChimesEnabled = entity.Api is ICoreClientAPI api && api.ModLoader.IsModEnabled("windchimes");
        }

        public override void OnGameTick(float deltaTime)
        {
            base.OnGameTick(deltaTime);
            if (!windChimesEnabled || entity.Api is not ICoreClientAPI api) return;
            if (!entity.Alive)
            {
                windChimeSounds?.Update(inv);
                return;
            }
            soundUpdateElapsed += deltaTime;
            if (soundUpdateElapsed < 0.25f) return;
            soundUpdateElapsed = 0;
            if (windChimeSounds == null && (inv == null || !inv.Any(slot =>
                slot.Itemstack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true))) return;
            windChimeSounds ??= new WindChimeAttachmentSounds(api, entity);
            windChimeSounds.Update(inv);
        }

        public override void OnEntityDespawn(EntityDespawnData despawn)
        {
            windChimeSounds?.Dispose();
            base.OnEntityDespawn(despawn);
        }

        protected override Shape addGearToShape(Shape entityShape, ItemSlot gearslot, string slotCode,
            string shapePathForLogging, ref bool shapeIsCloned, ref string[] willDeleteElements,
            Dictionary<string, StepParentElementTo> overrideStepParent = null)
        {
            // The entity shader treats wind mode 9 as a frost flag and changes its movement.
            // The optional renderer draws these blocks separately with their original wind flags.
            if (windChimesEnabled && entity.Properties?.Client?.Renderer is WindChimeScarecrowRenderer
                && gearslot.Itemstack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true)
                return entityShape;
            return base.addGearToShape(entityShape, gearslot, slotCode, shapePathForLogging,
                ref shapeIsCloned, ref willDeleteElements, overrideStepParent);
        }
    }
}

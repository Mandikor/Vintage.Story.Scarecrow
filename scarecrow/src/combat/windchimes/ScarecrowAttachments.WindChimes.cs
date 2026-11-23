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
            // The container calls us with "default"; EntityBehaviorAttachable resolves the slot
            // name only inside base.addGearToShape. Inspect the actual inventory slot here so
            // separately rendered arm lights/tools are not also inserted into the body mesh.
            if (UsesSeparateArmRenderer() && IsArmInventorySlot(gearslot)
                && (ScarecrowPendulum.IsWindChime(gearslot.Itemstack)
                    || AnimatedArmLantern.Matches(gearslot.Itemstack)))
                return entityShape;
            string armCode = GetArmSlotCode(gearslot);
            ModelTransform neckTransform = IsNeckInventorySlot(gearslot)
                ? ScarecrowArmTransform.GetNeckTransform(gearslot.Itemstack) : null;
            if (entityShape == null || (neckTransform == null
                && ScarecrowArmTransform.GetMatrix(gearslot.Itemstack, armCode) == null))
                return base.addGearToShape(entityShape, gearslot, slotCode, shapePathForLogging,
                    ref shapeIsCloned, ref willDeleteElements, overrideStepParent);
            if (!shapeIsCloned) { entityShape = entityShape.Clone(); shapeIsCloned = true; }
            var previous = ScarecrowArmTransform.Snapshot(entityShape);
            Shape result = base.addGearToShape(entityShape, gearslot, slotCode, shapePathForLogging,
                ref shapeIsCloned, ref willDeleteElements, overrideStepParent);
            if (neckTransform != null) ScarecrowArmTransform.ApplyToAdded(result, previous, neckTransform);
            else ScarecrowArmTransform.ApplyToAdded(result, previous, gearslot.Itemstack, armCode);
            return result;
        }

        private bool IsArmInventorySlot(ItemSlot slot) => GetArmSlotCode(slot) != null;

        private bool UsesSeparateArmRenderer()
        {
            var client = entity.Properties?.Client;
            if (client?.Renderer is WindChimeScarecrowRenderer) return true;
            // The server also assembles the body shape, but never creates a client renderer.
            // Keep private arm meshes out there too: merging source models can duplicate
            // attachment codes or resolve their internal step parents against the body.
            // Body selection points and the little scarecrow's static neck gear stay intact.
            return entity.World?.Side == EnumAppSide.Server
                && (client?.RendererName == "ScarecrowAttachmentShape"
                    || client?.RendererName == "ScarecrowWindChimeShape");
        }

        private bool IsNeckInventorySlot(ItemSlot slot)
        {
            int index = System.Array.IndexOf(slotCodes, "sc-neck");
            return inv != null && index >= 0 && index < inv.Count && ReferenceEquals(inv[index], slot);
        }

        private string GetArmSlotCode(ItemSlot slot)
        {
            if (inv == null) return null;
            for (int i = 0; i < inv.Count && i < slotCodes.Length; i++)
                if (ReferenceEquals(inv[i], slot)) return ScarecrowArmTransform.AttributeForSlot(slotCodes[i]) != null ? slotCodes[i] : null;
            return null;
        }
    }
}

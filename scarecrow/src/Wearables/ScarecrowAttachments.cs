using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Vanilla attachment handling with an additional item-code filter per slot.</summary>
    public class ScarecrowAttachments : EntityBehaviorRideableAccessories
    {
        private string[][] allowedCodes = Array.Empty<string[]>();
        private string[][] categories = Array.Empty<string[]>();
        private int[] lanternSlotIndices = Array.Empty<int>();
        private string[] slotCodes = Array.Empty<string>();
        private WindChimeAttachmentSounds windChimeSounds;
        private bool windChimesEnabled;
        private float soundUpdateElapsed;

        public int LanternRadiusBonus => EquipmentRadiusBonus;

        public int EquipmentRadiusBonus
        {
            get
            {
                GetStrongestRadiusEquipment(out int bonus);
                return bonus;
            }
        }

        public ItemStack GetStrongestRadiusEquipment(out int bonus)
        {
            bonus = 0;
            ItemStack strongest = null;
            if (inv == null) return null;
            foreach (int index in lanternSlotIndices)
            {
                if (index >= inv.Count) continue;
                ItemStack stack = inv[index].Itemstack;
                AssetLocation code = stack?.Collectible?.Code;
                if (code == null || !allowedCodes[index].Any(pattern => WildcardUtil.Match(pattern, code.ToString()))) continue;
                int candidate = GetEquipmentRadiusBonus(stack);
                if (candidate <= bonus) continue;
                bonus = candidate;
                strongest = stack;
            }
            return strongest;
        }

        private static int GetEquipmentRadiusBonus(ItemStack stack)
        {
            AssetLocation code = stack.Collectible.Code;
            if (code.Domain == "game" && code.Path.EndsWith("-up", StringComparison.Ordinal))
            {
                if (code.Path.StartsWith("lantern-small-", StringComparison.Ordinal)) return 2;
                if (code.Path.StartsWith("lantern-large-", StringComparison.Ordinal)) return 4;
            }
            // Optional compatibility patches supply their bonus without hard-coded mod IDs.
            return Math.Clamp(stack.ItemAttributes?["scarecrowRadiusBonus"].AsInt(0) ?? 0, 0, 6);
        }

        public ScarecrowAttachments(Entity entity) : base(entity) { }

        private JsonObject PrepareConfig(JsonObject config, IWorldAccessor world)
        {
            // Respect the actual categories of permitted collectibles, including other mods.
            JsonObject copy = new JsonObject(config.Token.DeepClone());
            JArray slots = (JArray)copy["wearableSlots"].Token;
            allowedCodes = new string[slots.Count][];
            categories = new string[slots.Count][];
            slotCodes = new string[slots.Count];
            List<int> lanternSlots = new();
            for (int i = 0; i < slots.Count; i++)
            {
                string slotCode = slots[i].Value<string>("code");
                slotCodes[i] = slotCode;
                if (slotCode is "sc-neck" or "sc-leftarm" or "sc-rightarm") lanternSlots.Add(i);
                allowedCodes[i] = slots[i]["allowedCodes"]?.ToObject<string[]>() ?? Array.Empty<string>();
                AssetLocation[] patterns = allowedCodes[i].Select(code => new AssetLocation(code)).ToArray();
                HashSet<string> accepted = new();
                foreach (CollectibleObject collectible in world.Collectibles)
                {
                    if (collectible?.Code == null || !patterns.Any(pattern => WildcardUtil.Match(pattern, collectible.Code))) continue;
                    string category = IAttachableToEntity.FromCollectible(collectible)?.GetCategoryCode(new ItemStack(collectible));
                    if (category != null) accepted.Add(category);
                }
                categories[i] = accepted.ToArray();
                slots[i]["forCategoryCodes"] = JArray.FromObject(categories[i]);
            }
            lanternSlotIndices = lanternSlots.ToArray();
            return copy;
        }

        public override void Initialize(EntityProperties properties, JsonObject attributes)
        {
            InitializeWindChimeSounds();
            base.Initialize(properties, PrepareConfig(attributes, entity.World));
            RestoreCarriedEquipment();
        }

        public const string StoredEquipmentAttribute = "scarecrowEquipment";

        public static bool HasStoredEquipment(ItemStack stack) =>
            stack?.Attributes?.GetTreeAttribute(StoredEquipmentAttribute)?.Any(entry =>
                entry.Value is ItemstackAttribute { value: not null }) == true;

        public void StoreEquipment(ItemStack stack)
        {
            if (inv == null || inv.Empty) return;
            TreeAttribute equipment = new();
            for (int i = 0; i < inv.Count && i < slotCodes.Length; i++)
            {
                if (!inv[i].Empty) equipment.SetItemstack(slotCodes[i], inv[i].Itemstack.Clone());
            }
            stack.Attributes[StoredEquipmentAttribute] = equipment;
        }

        public void ClearEquipmentAfterPickup()
        {
            if (inv == null || inv.Empty) return;
            foreach (ItemSlot slot in inv) slot.Itemstack = null;
            storeInv();
        }

        private void RestoreCarriedEquipment()
        {
            ITreeAttribute equipment = entity.Attributes.GetTreeAttribute(StoredEquipmentAttribute);
            if (equipment == null) return;
            for (int i = 0; i < inv.Count && i < slotCodes.Length; i++)
            {
                ItemStack stack = equipment.GetItemstack(slotCodes[i])?.Clone();
                if (stack == null) continue;
                if (stack.ResolveBlockOrItem(entity.World)) inv[i].Itemstack = stack;
                else entity.World.Logger.Warning("Scarecrow: could not restore missing equipment in slot {0}.", slotCodes[i]);
            }
            entity.Attributes.RemoveAttribute(StoredEquipmentAttribute);
            storeInv();
        }

        private void InitializeWindChimeSounds()
        {
            windChimeSounds?.Dispose();
            windChimeSounds = null;
            soundUpdateElapsed = 0;
            windChimesEnabled = entity.Api is ICoreClientAPI api && api.ModLoader.IsModEnabled("windchimes");
        }

        public override bool TryEarlyLoadCollectibleMappings(IWorldAccessor world,
            Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping,
            bool resolveImports, EntityProperties entityProperties, JsonObject behaviorConfig)
        {
            return base.TryEarlyLoadCollectibleMappings(world, oldBlockIdMapping, oldItemIdMapping,
                resolveImports, entityProperties, PrepareConfig(behaviorConfig, world));
        }

        protected override void loadInv()
        {
            base.loadInv();
            if (inv == null) return;
            for (int i = 0; i < inv.Count && i < allowedCodes.Length; i++)
            {
                if (inv[i] is ScarecrowAttachmentSlot) continue;
                // Loading is not insertion: keep saved contents removable after rule changes.
                ItemStack stack = inv[i].Itemstack;
                inv[i] = new ScarecrowAttachmentSlot(inv, categories[i], allowedCodes[i]) { Itemstack = stack };
            }
            windChimeSounds?.Update(inv);
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

        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            windChimeSounds?.Dispose();
            base.OnEntityDeath(damageSourceForDeath);
            // Vanilla DropAll empties the inventory, but only marks the mesh dirty.
            // Publish the final empty snapshot while the corpse remains in the world.
            if (entity.World.Side == EnumAppSide.Server && inv != null) storeInv();
            entity.MarkShapeModified();
        }

        public override void OnTesselation(ref Shape entityShape, string shapePathForLogging,
            ref bool shapeIsCloned, ref string[] willDeleteElements)
        {
            if (!entity.Alive) return;
            base.OnTesselation(ref entityShape, shapePathForLogging, ref shapeIsCloned, ref willDeleteElements);
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

        public override void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition,
            EnumInteractMode mode, ref EnumHandling handled)
        {
            if (mode == EnumInteractMode.Interact && entity is EntityScarecrow scarecrow
                && !scarecrow.TryManage((byEntity as EntityPlayer)?.Player))
            {
                handled = EnumHandling.PreventSubsequent;
                return;
            }
            // Vanilla only transfers the item on the server. Check the whitelist here as well
            // so the client cannot predict a successful attachment for a forbidden tool.
            EntityControls controls = byEntity.MountedOn?.Controls ?? byEntity.Controls;
            int boxIndex = (byEntity as EntityPlayer)?.EntitySelection?.SelectionBoxIndex ?? -1;
            if (mode == EnumInteractMode.Interact && !itemslot.Empty && boxIndex > 0
                && (UseShiftAttach ? controls.ShiftKey : controls.CtrlKey)
                && GetSlotFromSelectionBoxIndex(boxIndex - 1) is ScarecrowAttachmentSlot slot)
            {
                string reason = !slot.Empty ? "occupied"
                    : !slot.CanHold(itemslot)
                        || IAttachableToEntity.FromCollectible(itemslot.Itemstack.Collectible)?.IsAttachable(entity, itemslot.Itemstack) != true
                        ? "equipment" : null;
                if (reason != null)
                {
                    InteractionFeedback.Show((byEntity as EntityPlayer)?.Player, reason);
                    handled = EnumHandling.PreventSubsequent;
                    return;
                }
            }
            base.OnInteract(byEntity, itemslot, hitPosition, mode, ref handled);
        }

        public override void OnReceivedClientPacket(IServerPlayer player, int packetid, byte[] data, ref EnumHandling handled)
        {
            if (entity is EntityScarecrow scarecrow && !scarecrow.TryManage(player))
            {
                handled = EnumHandling.PreventSubsequent;
                return;
            }
            base.OnReceivedClientPacket(player, packetid, data, ref handled);
        }

        public override WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection selection,
            IClientPlayer player, ref EnumHandling handled)
        {
            if (!entity.Alive) return null;
            WorldInteraction[] help = base.GetInteractionHelp(world, selection, player, ref handled);
            if (help == null || selection.SelectionBoxIndex <= 0) return help;
            if (GetSlotFromSelectionBoxIndex(selection.SelectionBoxIndex - 1) is not ScarecrowAttachmentSlot slot) return help;

            // Vanilla caches these objects. Filter copies, never the shared cache.
            return help.Select(interaction => new WorldInteraction
            {
                ActionLangCode = interaction.ActionLangCode,
                MouseButton = interaction.MouseButton,
                HotKeyCode = interaction.HotKeyCode,
                HotKeyCodes = interaction.HotKeyCodes,
                RequireFreeHand = interaction.RequireFreeHand,
                JsonItemStacks = interaction.JsonItemStacks,
                GetMatchingStacks = interaction.GetMatchingStacks,
                ShouldApply = interaction.ShouldApply,
                Itemstacks = interaction.Itemstacks?.Where(slot.Allows).ToArray()
            }).Where(interaction => interaction.Itemstacks == null || interaction.Itemstacks.Length > 0).ToArray();
        }
    }
}

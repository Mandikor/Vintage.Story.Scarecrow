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
    public partial class ScarecrowAttachments : EntityBehaviorRideableAccessories
    {
        private string[][] allowedCodes = Array.Empty<string[]>();
        private string[][] categories = Array.Empty<string[]>();
        private int[] lanternSlotIndices = Array.Empty<int>();
        private string[] slotCodes = Array.Empty<string>();

        // Radius bonuses use the strongest permitted attachment, never a sum.
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
                if (code == null) continue;
                bool legacyNeck = slotCodes[index] == "sc-neck" && allowedCodes[index].Length == 0
                    && ScarecrowPendulum.IsLegacyHanger(stack);
                if (!legacyNeck && !allowedCodes[index].Any(pattern => WildcardUtil.Match(pattern, code.ToString()))) continue;
                int candidate = GetEquipmentRadiusBonus(stack);
                if (candidate <= bonus) continue;
                bonus = candidate;
                strongest = stack;
            }
            return strongest;
        }

        internal static int GetEquipmentRadiusBonus(ItemStack stack)
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

        // Inventory details use stable semantic slot codes, never selection-box names.
        // Omit absent/empty slots so the little scarecrow cannot advertise arm storage.
        public IEnumerable<(string Code, ItemStack Stack)> GetOccupiedSlots()
        {
            if (inv == null) yield break;
            foreach (string code in new[] { "sc-leftarm", "sc-rightarm", "sc-neck", "sc-foot" })
            {
                int index = Array.IndexOf(slotCodes, code);
                if (index >= 0 && index < inv.Count && !inv[index].Empty)
                    yield return (code, inv[index].Itemstack);
            }
        }

        // Build a per-entity config from resolved collectible categories; never alter shared assets.
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

        // Stable slot codes allow equipment to round-trip through a picked-up item.
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
            if (entity.World.Side == EnumAppSide.Server) MoveLegacyNeckLantern();
            storeInv();
        }

        // Keep inventory indices stable for existing worlds and packed items. A disabled neck slot
        // remains removable storage if both arms are full; never overwrite or drop its contents.
        private bool MoveLegacyNeckLantern()
        {
            int neck = Array.IndexOf(slotCodes, "sc-neck");
            if (neck < 0 || neck >= inv.Count || allowedCodes[neck].Length != 0
                || !ScarecrowPendulum.IsLegacyHanger(inv[neck].Itemstack)) return false;
            for (int i = 0; i < inv.Count && i < slotCodes.Length; i++)
            {
                if (slotCodes[i] is not ("sc-leftarm" or "sc-rightarm") || !inv[i].Empty
                    || inv[i] is not ScarecrowAttachmentSlot arm || !arm.Allows(inv[neck].Itemstack)) continue;
                inv[i].Itemstack = inv[neck].Itemstack;
                inv[neck].Itemstack = null;
                return true;
            }
            return false;
        }

        // Saved inventories need the same slot filters during import and normal loading.
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
            if (entity.World.Side == EnumAppSide.Server && MoveLegacyNeckLantern())
            {
                storeInv();
                entity.MarkShapeModified();
            }
            RefreshCompatibilitySounds();
        }

        // Death uses vanilla drops, then publishes the emptied inventory to connected clients.
        public override void OnEntityDeath(DamageSource damageSourceForDeath)
        {
            DisposeCompatibilitySounds();
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

        // Enforce ownership and the whitelist before vanilla predicts or transfers equipment.
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

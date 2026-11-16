using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Scarecrow
{
    /// <summary>
    /// Shared placement logic for the scarecrow items.
    /// </summary>
    public abstract class ItemScarecrowBase : Item
    {
        private static readonly string[] EquipmentSlotOrder = { "sc-leftarm", "sc-rightarm", "sc-neck", "sc-foot" };

        protected virtual bool HasArmSlots => true;

        /// <summary>Entity spawned when this item is placed.</summary>
        protected abstract string EntityCode { get; }

        /// <summary>Lang key of the "place" interaction help.</summary>
        protected abstract string PlaceHelpLangCode { get; }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (blockSel == null || !(byEntity is EntityPlayer entityPlayer)) return;

            IPlayer player = byEntity.World.PlayerByUid(entityPlayer.PlayerUID);
            if (player == null) return;

            // Clicking a face places into the neighbouring block, DidOffset means the
            // selection was offset onto the block itself.
            BlockPos targetPos = blockSel.DidOffset
                ? blockSel.Position
                : blockSel.Position.AddCopy(blockSel.Face);

            if (!byEntity.World.Claims.TryAccess(player, targetPos, EnumBlockAccessFlags.BuildOrBreak))
            {
                slot.MarkDirty();
                return;
            }

            EntityProperties entityType = byEntity.World.GetEntityType(new AssetLocation(EntityCode));
            if (entityType == null)
            {
                byEntity.World.Logger.Error("Scarecrow: entity type '{0}' not found, cannot place.", EntityCode);
                return;
            }

            Entity entity = byEntity.World.ClassRegistry.CreateEntity(entityType);
            if (entity == null) return;

            // Center the entity in the target block instead of using the raw hit point.
            entity.Pos.X = targetPos.X + 0.5;
            entity.Pos.Y = targetPos.Y;
            entity.Pos.Z = targetPos.Z + 0.5;
            entity.Pos.Yaw = byEntity.Pos.Yaw + GameMath.PIHALF;
            if (entity is EntityScarecrow scarecrow) scarecrow.AssignOwner(player);

            // The entity has not been initialized yet, so use the type's spawn box.
            Cuboidf collisionBox = entityType.SpawnCollisionBox;
            if (byEntity.World.CollisionTester.IsColliding(byEntity.World.BlockAccessor,
                collisionBox, entity.Pos.XYZ, false))
            {
                handling = EnumHandHandling.PreventDefaultAction;
                return;
            }

            // This helper expects boxes relative to the block, rather than the entity centre.
            Cuboidf blockRelativeBox = collisionBox.Clone().Translate(0.5f, 0, 0.5f);
            Entity[] intersectingEntities = byEntity.World.GetIntersectingEntities(targetPos,
                new[] { blockRelativeBox }, other => other is EntityAgent && other.Alive);
            // No intersections are returned as an empty array, not null.
            if (intersectingEntities.Length > 0)
            {
                handling = EnumHandHandling.PreventDefaultAction;
                return;
            }

            // Only consume the item once the entity was created and the space is clear.
            var equipment = slot.Itemstack.Attributes.GetTreeAttribute(ScarecrowAttachments.StoredEquipmentAttribute);
            if (equipment != null)
                entity.Attributes[ScarecrowAttachments.StoredEquipmentAttribute] = equipment.Clone();

            if (player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }

            byEntity.World.PlaySoundAt(new AssetLocation("game:sounds/block/torch"), entity, player);
            byEntity.World.SpawnEntity(entity);

            handling = EnumHandHandling.PreventDefaultAction;
        }

        public override int GetMergableQuantity(ItemStack sinkStack, ItemStack sourceStack, EnumMergePriority priority)
        {
            if (ScarecrowAttachments.HasStoredEquipment(sinkStack) || ScarecrowAttachments.HasStoredEquipment(sourceStack))
                return 0;
            return base.GetMergableQuantity(sinkStack, sourceStack, priority);
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
            if (!ScarecrowAttachments.HasStoredEquipment(inSlot.Itemstack)) return;

            var equipment = inSlot.Itemstack.Attributes.GetTreeAttribute(ScarecrowAttachments.StoredEquipmentAttribute);
            if (dsc.Length > 0 && dsc[dsc.Length - 1] != '\n') dsc.AppendLine();
            dsc.AppendLine();
            dsc.AppendLine("<strong>" + Lang.Get("scarecrow:tooltip-inventory") + "</strong>");
            foreach (string code in EquipmentSlotOrder)
            {
                if (!HasArmSlots && (code == "sc-leftarm" || code == "sc-rightarm")) continue;
                // Tooltip rendering must not resolve or otherwise modify the stored inventory itself.
                ItemStack stack = equipment.GetItemstack(code)?.Clone();
                string name = stack == null ? Lang.Get("scarecrow:tooltip-inventory-empty")
                    : stack.ResolveBlockOrItem(world) ? stack.GetName() : Lang.Get("scarecrow:tooltip-inventory-unavailable");
                name = name.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                dsc.AppendLine(Lang.Get("scarecrow:tooltip-inventory-" + code, name));
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new()
                {
                    ActionLangCode = PlaceHelpLangCode,
                    MouseButton = EnumMouseButton.Right
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
}

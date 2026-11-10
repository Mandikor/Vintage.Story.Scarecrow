using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
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
            entity.WatchedAttributes.SetString("ownerUid", player.PlayerUID);

            // Only consume the item once the entity was created successfully.
            if (player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }

            byEntity.World.PlaySoundAt(new AssetLocation("game:sounds/block/torch"), entity, player);
            byEntity.World.SpawnEntity(entity);

            handling = EnumHandHandling.PreventDefaultAction;
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

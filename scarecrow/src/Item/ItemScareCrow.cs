namespace Scarecrow
{
    public class ItemScareCrow : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (blockSel == null) return;

            IWorldAccessor world = byEntity.World;
            EntityPlayer entityPlayer = byEntity as EntityPlayer;
            IPlayer player = world.PlayerByUid(entityPlayer?.PlayerUID);

            if (!byEntity.World.Claims.TryAccess(player, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                slot.MarkDirty();
                return;
            }

            if (byEntity is not EntityPlayer || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }

            EntityProperties entityType = byEntity.World.GetEntityType(new AssetLocation("scarecrow:scarecrow"));
            Entity entity = byEntity.World.ClassRegistry.CreateEntity(entityType);

            if (entity != null)
            {
                entity.ServerPos.X = blockSel.Position.X + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.X) + 0.5f;
                entity.ServerPos.Y = blockSel.Position.Y + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.Y);
                entity.ServerPos.Z = blockSel.Position.Z + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.Z) + 0.5f;
                entity.ServerPos.Yaw = byEntity.SidedPos.Yaw + GameMath.PI;

                if (player != null && player.PlayerUID != null)
                {
                    entity.WatchedAttributes.SetString("ownerUid", player.PlayerUID);
                }

                entity.Pos.SetFrom(entity.ServerPos);
                byEntity.World.PlaySoundAt(new AssetLocation("game:sounds/block/torch"), entity, player, true, 32f, 1f);
                byEntity.World.SpawnEntity(entity);
                handling = EnumHandHandling.PreventDefaultAction;
            }
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new() {
                    ActionLangCode = "scarecrow:heldhelp-place-sc",
                    MouseButton = EnumMouseButton.Right
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
}

using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

#nullable disable

namespace Scarecrow
{
    public class EntityLittleScareCrow : EntityHumanoid
    {
        private ICoreServerAPI sapi;
        public bool DebugOutput { get; set; } = false;

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
                sapi.Event.OnEntitySpawn += Event_EntitySpawn;
            }
            else
            {
                ICoreClientAPI capi = api as ICoreClientAPI;
            }
        }

        private void Event_EntitySpawn(Entity entity)
        {
            if (entity.Code.Path.StartsWith("hare") || entity.Code.Path.StartsWith("raccoon"))
            {
                double distance = this.Pos.DistanceTo(entity.Pos);
                if (distance <= 8)
                {
                    if (DebugOutput)
                    {
                        sapi.Logger.Debug($"Scarecrow: EntitySpawn: Blocking {entity.Code} at {distance:N0} blocks away.");
                    }
                    entity.Die(EnumDespawnReason.Removed);
                }
            }
            return;
        }

        /// <summary>
        /// Blocks only hares and raccoons in range.
        /// </summary>
        /// <param name="entityProperties"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="herdId"></param>
        /// <returns></returns>
        public bool SpawnInterceptor(IBlockAccessor blockAccessor, ref EntityProperties entityProperties, Vec3d spawnPosition, long herdId)
        {
            if (entityProperties.Code.Path.StartsWith("hare") || entityProperties.Code.Path.StartsWith("raccoon"))
            {
                double distance = this.Pos.DistanceTo(spawnPosition);
                if (distance <= 8)
                {
                    if (DebugOutput)
                    {
                        sapi.Logger.Debug($"Scarecrow: Blocking {entityProperties.Code} at {distance:N0} blocks away.");
                    }
                    return false;
                }
            }
            return true;
        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            if (!Api.World.Claims.TryAccess(((EntityPlayer)byEntity).Player, Pos.AsBlockPos, EnumBlockAccessFlags.Use) || !Alive || World.Side == EnumAppSide.Client || mode == 0)
            {
                base.OnInteract(byEntity, slot, hitPosition, mode);
                return;
            }

            string owneruid = WatchedAttributes.GetString("ownerUid", null);
            string agentUid = (byEntity as EntityPlayer)?.PlayerUID;

            if (agentUid != null && (owneruid == null || owneruid == "" || owneruid == agentUid) && byEntity.Controls.CtrlKey && byEntity.RightHandItemSlot.Empty)
            {
                ItemStack itemStack = new(byEntity.World.GetItem(new AssetLocation("scarecrow:little-scarecrow")));

                if (!byEntity.TryGiveItemStack(itemStack))
                {
                    byEntity.World.SpawnItemEntity(itemStack, Pos.XYZ);
                }

                if (Api.Side == EnumAppSide.Server)
                {
                    sapi.Event.OnTrySpawnEntity -= SpawnInterceptor;
                    sapi.Event.OnEntitySpawn -= Event_EntitySpawn;
                }

                byEntity.World.Logger.Audit("{0} Took 1x {1} at {2}.",
                    byEntity.GetName(),
                    itemStack.Collectible.Code,
                    Pos.AsBlockPos
                );

                Die();
                return;
            }

            base.OnInteract(byEntity, slot, hitPosition, mode);
        }

        public override WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player)
        {
            var interactions = ObjectCacheUtil.GetOrCreate(world.Api, "scarecrowInteractions" + EntityId, () =>
            {
                return new WorldInteraction[] {
                    new()
                    {
                        ActionLangCode = "scarecrow:entityhelp-pickup",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "ctrl",
                        RequireFreeHand = true
                    }
                };
            });
            return interactions.Append(base.GetInteractionHelp(world, es, player));
        }
    }
}
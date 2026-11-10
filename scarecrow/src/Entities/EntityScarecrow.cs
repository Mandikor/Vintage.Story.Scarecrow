using System;
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
    /// <summary>
    /// Scarecrow-like entity, used for the scarecrow, the little scarecrow and the vanilla straw dummy.
    /// The differences between them are data driven through the entity attributes
    /// "scareRadius" (blocks) and "pickupItemCode" (item returned when picked up).
    /// </summary>
    public class EntityScarecrow : EntityHumanoid
    {
        private const int DefaultScareRadius = 8;
        private const string DefaultPickupItemCode = "game:strawdummy";

        private ICoreServerAPI sapi;
        private ScarecrowConfig config;
        private bool spawnHooksAttached;
        private int scareRadius = DefaultScareRadius;
        private string pickupItemCode = DefaultPickupItemCode;

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            config = api.ModLoader.GetModSystem<Core>()?.Config ?? new ScarecrowConfig();
            scareRadius = properties.Attributes?["scareRadius"]?.AsInt(DefaultScareRadius) ?? DefaultScareRadius;
            pickupItemCode = properties.Attributes?["pickupItemCode"]?.AsString(DefaultPickupItemCode) ?? DefaultPickupItemCode;

            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                AttachSpawnHooks();
            }
        }

        public override void OnEntityDespawn(EntityDespawnData despawnData)
        {
            DetachSpawnHooks();
            base.OnEntityDespawn(despawnData);
        }

        /// <summary>
        /// Subscribes to the server spawn events. Safe to call more than once.
        /// </summary>
        private void AttachSpawnHooks()
        {
            if (spawnHooksAttached || sapi == null) return;

            sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
            sapi.Event.OnEntitySpawn += Event_EntitySpawn;
            spawnHooksAttached = true;
        }

        /// <summary>
        /// Unsubscribes from the server spawn events. Safe to call more than once.
        /// </summary>
        private void DetachSpawnHooks()
        {
            if (!spawnHooksAttached || sapi == null) return;

            sapi.Event.OnTrySpawnEntity -= SpawnInterceptor;
            sapi.Event.OnEntitySpawn -= Event_EntitySpawn;
            spawnHooksAttached = false;
        }

        private void Event_EntitySpawn(Entity entity)
        {
            if (!IsScaredAnimal(entity.Code)) return;

            double distance = Pos.DistanceTo(entity.Pos);
            if (distance > scareRadius) return;

            if (config?.DebugOutput == true)
            {
                sapi.Logger.Debug($"Scarecrow: EntitySpawn: Blocking {entity.Code} at {distance:N0} blocks away.");
            }

            entity.Die(EnumDespawnReason.Removed);
        }

        /// <summary>
        /// Blocks the configured animal prefixes in range.
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="entityProperties"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="herdId"></param>
        /// <returns></returns>
        public bool SpawnInterceptor(IBlockAccessor blockAccessor, ref EntityProperties entityProperties, Vec3d spawnPosition, long herdId)
        {
            if (!IsScaredAnimal(entityProperties.Code)) return true;

            double distance = Pos.DistanceTo(spawnPosition);
            if (distance > scareRadius) return true;

            if (config?.DebugOutput == true)
            {
                sapi.Logger.Debug($"Scarecrow: Blocking {entityProperties.Code} at {distance:N0} blocks away.");
            }

            return false;
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
                Item item = byEntity.World.GetItem(new AssetLocation(pickupItemCode));
                if (item == null)
                {
                    byEntity.World.Logger.Error("Scarecrow: pickup item '{0}' not found, entity cannot be picked up.", pickupItemCode);
                    base.OnInteract(byEntity, slot, hitPosition, mode);
                    return;
                }

                ItemStack itemStack = new(item);

                if (!byEntity.TryGiveItemStack(itemStack))
                {
                    byEntity.World.SpawnItemEntity(itemStack, Pos.XYZ);
                }

                DetachSpawnHooks();

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
            var interactions = ObjectCacheUtil.GetOrCreate(world.Api, "scarecrow:interactions:pickup", () =>
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

        private bool IsScaredAnimal(AssetLocation code)
        {
            string path = code?.Path;
            if (path == null) return false;

            string[] prefixes = config?.ScaredAnimalPrefixes;
            if (prefixes == null) return false;

            foreach (string prefix in prefixes)
            {
                if (!string.IsNullOrEmpty(prefix) && path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }
    }
}
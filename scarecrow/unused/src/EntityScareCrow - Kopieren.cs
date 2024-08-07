using Vintagestory.API.Server;

namespace Scarecrow
{
    public class EntityScareCrow : EntityHumanoid
    {
        public static Config Config { get; set; }

        private ICoreServerAPI sapi;

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
            }
            else
            {
                ICoreClientAPI capi = api as ICoreClientAPI;

            }
        }

        /// <summary>
        /// Only block hares and raccoons in range.
        /// </summary>
        /// <param name="entityProperties"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="herdId"></param>
        /// <returns>true / false</returns>
        public bool SpawnInterceptor(IBlockAccessor blockAccessor, ref EntityProperties entityProperties, Vec3d spawnPosition, long herdId)
        {
            if (entityProperties.Code.Path.StartsWithFast("hare"))
            {
                double distance = ServerPos.DistanceTo(spawnPosition);
                if (distance <= Config.BlockRadiusScarecrowHare)
                {
                    return false;
                }
            }

            if (entityProperties.Code.Path.StartsWithFast("raccoon"))
            {
                double distance = ServerPos.DistanceTo(spawnPosition);
                if (distance <= Config.BlockRadiusScarecrowRaccoon)
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            if (!Alive || World.Side == EnumAppSide.Client || mode == 0)
            {
                base.OnInteract(byEntity, slot, hitPosition, mode);
                return;
            }

            string owneruid = WatchedAttributes.GetString("ownerUid", null);
            string agentUid = (byEntity as EntityPlayer)?.PlayerUID;

            if (agentUid != null && (owneruid == null || owneruid == "" || owneruid == agentUid) && byEntity.Controls.CtrlKey)
            {
                ItemStack itemStack = new(byEntity.World.GetItem(new AssetLocation("scarecrow:scarecrow")), 1);

                if (!byEntity.TryGiveItemStack(itemStack))
                {
                    byEntity.World.SpawnItemEntity(itemStack, ServerPos.XYZ, null);
                }

                if (Api.Side == EnumAppSide.Server)
                {
                    sapi.Event.OnTrySpawnEntity -= SpawnInterceptor;
                }

                Die(EnumDespawnReason.Death, null);
                return;
            }

            base.OnInteract(byEntity, slot, hitPosition, mode);
        }

        public override WorldInteraction[] GetInteractionHelp(IClientWorldAccessor world, EntitySelection es, IClientPlayer player)
        {
            var interactions = ObjectCacheUtil.GetOrCreate(world.Api, "scarecrowInteractions" + EntityId, () =>
            {

                List<ItemStack> knifeStacklist = new();

                foreach (Item item in world.Api.World.Items)
                {
                    if (item.Code == null) continue;
                    if (item.Tool == EnumTool.Knife)
                    {
                        knifeStacklist.Add(new ItemStack(item));
                    }
                }

                return new WorldInteraction[] {
                    new()
                    {
                        ActionLangCode = Code.Domain + ":entityhelp-pickup",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "ctrl"
                    //},
                    //new()
                    //{
                    //    ActionLangCode = Code.Domain + ":entityhelp-downgrade-sc",
                    //    MouseButton = EnumMouseButton.Right,
                    //    HotKeyCode = "ctrl",
                    //    Itemstacks = knifeStacklist.ToArray()
                    }
                };
            });
            return interactions.Append(base.GetInteractionHelp(world, es, player));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Scarecrow;

public class EntityScareCrow : EntityHumanoid
{
    private ICoreServerAPI sapi;
    private Config scconfig;

    public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
    {
        base.Initialize(properties, api, InChunkIndex3d);
        if (api.Side == EnumAppSide.Server)
        {
            sapi = api as ICoreServerAPI;
            sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
            sapi.Event.OnEntitySpawn += Event_EntitySpawn;
            scconfig = api.ModLoader.GetModSystem<Core>(true).SCConfig;
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
            double distance = this.ServerPos.DistanceTo(entity.ServerPos);
            if (distance <= scconfig.BlockRadiusScarecrow)
            {
                if (scconfig.DebugOutput)
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
            double distance = this.ServerPos.DistanceTo(spawnPosition);
            if (distance <= scconfig.BlockRadiusScarecrow)
            {
                if (scconfig.DebugOutput)
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
        if (!Alive || World.Side == EnumAppSide.Client || mode == EnumInteractMode.Attack)
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
                sapi.Event.OnEntitySpawn -= Event_EntitySpawn;
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
                    ActionLangCode = "scarecrow:entityhelp-pickup",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "ctrl"
                }
            };
        });
        return interactions.Append(base.GetInteractionHelp(world, es, player));
    }
}
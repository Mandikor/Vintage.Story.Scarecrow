using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
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
        private const string SpawnPrefixesAttribute = "scarecrowSpawnPrefixes";
        private const string OwnerNameAttribute = "scarecrowOwnerName";

        private ICoreServerAPI sapi;
        private ScarecrowConfig config;
        private bool spawnHooksAttached;
        private int scareRadius = DefaultScareRadius;
        private string pickupItemCode = DefaultPickupItemCode;
        private readonly Dictionary<string, string> animalGroupNames = new();

        /// <summary>Natural-spawn protection, including the strongest attached equipment bonus.</summary>
        public int ScareRadius => scareRadius + (GetBehavior<ScarecrowAttachments>()?.EquipmentRadiusBonus ?? 0);

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            config = api.ModLoader.GetModSystem<Core>()?.Config ?? new ScarecrowConfig();
            scareRadius = properties.Attributes?["scareRadius"]?.AsInt(DefaultScareRadius) ?? DefaultScareRadius;
            pickupItemCode = properties.Attributes?["pickupItemCode"]?.AsString(DefaultPickupItemCode) ?? DefaultPickupItemCode;

            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                SynchronizeOwnerName();
                SynchronizeProtectionSettings();
                AttachSpawnHooks();
            }
        }

        public override void OnEntityDespawn(EntityDespawnData despawnData)
        {
            DetachSpawnHooks();
            base.OnEntityDespawn(despawnData);
        }

        private void SynchronizeProtectionSettings()
        {
            WatchedAttributes.SetStringArray(SpawnPrefixesAttribute,
                config?.ScaredAnimalPrefixes ?? Array.Empty<string>());
        }

        public void AssignOwner(IPlayer player)
        {
            WatchedAttributes.SetString("ownerUid", player.PlayerUID);
            if (!string.IsNullOrEmpty(player.PlayerName))
                WatchedAttributes.SetString(OwnerNameAttribute, player.PlayerName);
        }

        private void SynchronizeOwnerName()
        {
            string uid = WatchedAttributes.GetString("ownerUid");
            if (string.IsNullOrEmpty(uid))
            {
                WatchedAttributes.RemoveAttribute(OwnerNameAttribute);
                return;
            }
            string name = World.PlayerByUid(uid)?.PlayerName
                ?? sapi?.PlayerData.GetPlayerDataByUid(uid)?.LastKnownPlayername;
            if (!string.IsNullOrEmpty(name)) WatchedAttributes.SetString(OwnerNameAttribute, name);
        }

        internal bool CanManage(IPlayer player)
            => CheckManagementAccess(player, false);

        internal bool TryManage(IPlayer player)
            => CheckManagementAccess(player, true);

        private bool CheckManagementAccess(IPlayer player, bool reportFailure)
        {
            if (!Alive || player == null) return false;
            string ownerUid = WatchedAttributes.GetString("ownerUid");
            string reason = !string.IsNullOrEmpty(ownerUid) && ownerUid != player.PlayerUID ? "owner"
                : !World.Claims.TryAccess(player, Pos.AsBlockPos, EnumBlockAccessFlags.Use) ? "claim" : null;
            if (reason == null) return true;
            if (reportFailure) InteractionFeedback.Show(player, reason);
            return false;
        }

        public override string GetInfoText()
        {
            StringBuilder text = new(base.GetInfoText());
            if (!Alive) return text.ToString();
            if (text.Length > 0 && text[text.Length - 1] != '\n') text.AppendLine();
            text.AppendLine();

            string owner = string.IsNullOrEmpty(WatchedAttributes.GetString("ownerUid"))
                ? Lang.Get("scarecrow:tooltip-owner-none")
                : WatchedAttributes.GetString(OwnerNameAttribute, Lang.Get("scarecrow:tooltip-owner-unknown"));
            text.AppendLine(FormatTooltipLine(0xADD8E6, "scarecrow:tooltip-owner", EscapeTooltipText(owner)));

            // The server owns spawn protection. Never substitute a client's local config.
            string[] prefixes = WatchedAttributes.GetStringArray(SpawnPrefixesAttribute, null);
            if (prefixes == null)
            {
                return text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-loading")).ToString();
            }
            prefixes = prefixes.Where(prefix => !string.IsNullOrEmpty(prefix))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (prefixes.Length == 0)
            {
                return text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-disabled")).ToString();
            }

            int bonus = 0;
            ItemStack equipment = GetBehavior<ScarecrowAttachments>()?.GetStrongestRadiusEquipment(out bonus);
            CultureInfo culture;
            try { culture = CultureInfo.GetCultureInfo(Lang.CurrentLocale); }
            catch (CultureNotFoundException) { culture = CultureInfo.InvariantCulture; }

            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-radius", (scareRadius + bonus).ToString("N0", culture)));
            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-radius-breakdown", scareRadius.ToString("N0", culture),
                equipment == null ? FormatProtectionLine("scarecrow:tooltip-equipment") : EscapeTooltipText(equipment.GetName()),
                bonus.ToString("N0", culture)));
            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-affected-animals",
                string.Join(Lang.Get("scarecrow:tooltip-list-separator"), prefixes.Select(GetAnimalGroupName).Distinct())));
            return text.ToString();
        }

        private string GetAnimalGroupName(string prefix)
        {
            string cacheKey = Lang.CurrentLocale + ":" + prefix;
            if (animalGroupNames.TryGetValue(cacheKey, out string cached)) return cached;
            string name = Lang.GetMatchingIfExists("scarecrow:tooltip-animal-" + prefix.ToLowerInvariant());
            if (name == null)
            {
                // Full entity codes can use the installed game/mod's creature translation.
                string[] names = World.EntityTypes.Where(type => type.Code?.Path.Equals(prefix, StringComparison.OrdinalIgnoreCase) == true)
                    .Select(type => Lang.GetMatchingIfExists(type.Code.Domain + ":item-creature-" + type.Code.Path))
                    .Where(value => value != null).Select(EscapeTooltipText).Distinct().ToArray();
                name = names.Length > 0 ? string.Join(Lang.Get("scarecrow:tooltip-list-separator"), names)
                    : Lang.Get("scarecrow:tooltip-code-prefix", EscapeTooltipText(prefix));
            }
            animalGroupNames[cacheKey] = name;
            return name;
        }

        private static string EscapeTooltipText(string value) => value.Replace("&", "&amp;")
            .Replace("<", "&lt;").Replace(">", "&gt;");

        private static string FormatProtectionLine(string key, params string[] values)
            => FormatTooltipLine(ScarecrowRadiusOverlay.RingColorRgb, key, values);

        private static string FormatTooltipLine(int color, string key, params string[] values)
        {
            string begin = "<font color=\"#" + color.ToString("X6", CultureInfo.InvariantCulture) + "\"><strong>";
            const string end = "</strong></font>";
            // Only translated fixed text is styled; substituted numbers and names keep normal formatting.
            return begin + Lang.Get(key, values.Select(value => end + value + begin).ToArray()) + end;
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

        /// <summary>
        /// Removes animals that the natural world spawner placed inside the radius.
        /// Newborn animals and animals spawned by players (for example when released
        /// from a cage) are left alone, they are not part of the natural spawn cycle
        /// and simply flee from the scarecrow.
        /// </summary>
        private void Event_EntitySpawn(Entity entity)
        {
            if (!IsScaredAnimal(entity.Code)) return;

            // The natural entity spawner marks its entities with this origin. Births
            // carry "reproduction" and player placements usually have no origin at all.
            string origin = entity.Attributes?.GetString("origin", null);
            if (origin != "entityspawner") return;

            double distance = Pos.DistanceTo(entity.Pos);
            if (distance > ScareRadius) return;

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
            if (distance > ScareRadius) return true;

            if (config?.DebugOutput == true)
            {
                sapi.Logger.Debug($"Scarecrow: Blocking {entityProperties.Code} at {distance:N0} blocks away.");
            }

            return false;
        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            // A denied interaction must never reach attachment handling in the base class.
            if (mode == EnumInteractMode.Interact && !TryManage((byEntity as EntityPlayer)?.Player)) return;

            if (!Alive || World.Side == EnumAppSide.Client || mode != EnumInteractMode.Interact)
            {
                base.OnInteract(byEntity, slot, hitPosition, mode);
                return;
            }

            if (byEntity.Controls.CtrlKey && byEntity.RightHandItemSlot.Empty)
            {
                Item item = byEntity.World.GetItem(new AssetLocation(pickupItemCode));
                if (item == null)
                {
                    byEntity.World.Logger.Error("Scarecrow: pickup item '{0}' not found, entity cannot be picked up.", pickupItemCode);
                    base.OnInteract(byEntity, slot, hitPosition, mode);
                    return;
                }

                ItemStack itemStack = CreatePickupStack(item);

                if (!ScarecrowPickup.TryGive(((EntityPlayer)byEntity).Player, itemStack))
                {
                    byEntity.World.SpawnItemEntity(itemStack, Pos.XYZ);
                }

                GetBehavior<ScarecrowAttachments>()?.ClearEquipmentAfterPickup();
                DetachSpawnHooks();

                byEntity.World.Logger.Audit("{0} Took 1x {1} at {2}.",
                    byEntity.GetName(),
                    itemStack.Collectible.Code,
                    Pos.AsBlockPos
                );

                Die(EnumDespawnReason.PickedUp);
                return;
            }

            base.OnInteract(byEntity, slot, hitPosition, mode);
        }

        private ItemStack CreatePickupStack(Item item)
        {
            ItemStack stack = new(item);
            GetBehavior<ScarecrowAttachments>()?.StoreEquipment(stack);
            return stack;
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

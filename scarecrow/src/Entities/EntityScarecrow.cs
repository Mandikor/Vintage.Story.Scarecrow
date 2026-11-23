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
    public partial class EntityScarecrow : EntityHumanoid
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
        private readonly Dictionary<string, AnimalSpawnRule> animalSpawnRules = new();

        /// <summary>Natural-spawn protection, including the strongest attached equipment bonus.</summary>
        public int ScareRadius => scareRadius + (GetBehavior<ScarecrowAttachments>()?.EquipmentRadiusBonus ?? 0);

        // Lifecycle: load data-driven settings and subscribe only while protection is active.
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
                if (Alive) AttachSpawnHooks();
            }
        }

        public override void OnEntityDespawn(EntityDespawnData despawnData)
        {
            DetachSpawnHooks();
            base.OnEntityDespawn(despawnData);
        }

        public override void Die(EnumDespawnReason reason = EnumDespawnReason.Death, DamageSource damageSourceForDeath = null)
        {
            DetachSpawnHooks();
            base.Die(reason, damageSourceForDeath);
        }

        private void SynchronizeProtectionSettings()
        {
            WatchedAttributes.SetStringArray(SpawnPrefixesAttribute,
                config?.ScaredAnimalPrefixes ?? Array.Empty<string>());
        }

        // Ownership is persisted by UID; display names are synchronized separately for tooltips.
        public void AssignOwner(IPlayer player)
        {
            // A newly placed or transferred scarecrow starts without inherited grants.
            WatchedAttributes.RemoveAttribute(SharedAccessAttribute);
            WatchedAttributes.MarkPathDirty(SharedAccessAttribute);
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
            => CheckManagementAccess(player, reportFailure, false);

        internal bool TryPickup(IPlayer player) => CheckManagementAccess(player, true, true);

        private bool CheckManagementAccess(IPlayer player, bool reportFailure, bool pickup)
        {
            if (!Alive || player == null) return false;
            string ownerUid = WatchedAttributes.GetString("ownerUid");
            string reason = !string.IsNullOrEmpty(ownerUid) && ownerUid != player.PlayerUID
                && !HasSharedAccess(player.PlayerUID, pickup) ? (pickup ? "pickup-access" : "owner")
                : !World.Claims.TryAccess(player, Pos.AsBlockPos, EnumBlockAccessFlags.Use) ? "claim" : null;
            if (reason == null) return true;
            if (reportFailure) InteractionFeedback.Show(player, reason);
            return false;
        }

        // Tooltips consume synchronized settings so clients show the server's actual protection.
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
            text.AppendLine();
            ScarecrowAttachments attachments = GetBehavior<ScarecrowAttachments>();

            // The server owns spawn protection. Never substitute a client's local config.
            string[] prefixes = WatchedAttributes.GetStringArray(SpawnPrefixesAttribute, null);
            if (prefixes == null)
            {
                text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-loading"));
                return AppendInventoryInfo(text, attachments).ToString();
            }
            prefixes = prefixes.Where(prefix => !string.IsNullOrEmpty(prefix) && GetAnimalSpawnRule(prefix).IsValid)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (prefixes.Length == 0)
            {
                text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-disabled"));
                return AppendInventoryInfo(text, attachments).ToString();
            }

            int bonus = attachments?.EquipmentRadiusBonus ?? 0;
            CultureInfo culture;
            try { culture = CultureInfo.GetCultureInfo(Lang.CurrentLocale); }
            catch (CultureNotFoundException) { culture = CultureInfo.InvariantCulture; }

            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-spawn-radius", (scareRadius + bonus).ToString("N0", culture)));
            // The vanilla straw dummy has no attachment inventory or equipment bonus.
            if (attachments != null)
                text.AppendLine(FormatProtectionLine("scarecrow:tooltip-radius-breakdown", scareRadius.ToString("N0", culture),
                    FormatProtectionLine("scarecrow:tooltip-equipment"),
                    bonus.ToString("N0", culture)));
            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-affected-animals",
                string.Join(Lang.Get("scarecrow:tooltip-list-separator"), prefixes.Select(GetAnimalGroupName).Distinct())));
            return AppendInventoryInfo(text, attachments).ToString();
        }

        private static StringBuilder AppendInventoryInfo(StringBuilder text, ScarecrowAttachments attachments)
        {
            var equipment = attachments?.GetOccupiedSlots().ToArray();
            if (equipment == null || equipment.Length == 0) return text;
            text.AppendLine();
            text.AppendLine(FormatProtectionLine("scarecrow:tooltip-inventory"));
            foreach (var slot in equipment)
            {
                string name = slot.Stack.Collectible != null ? slot.Stack.GetName()
                    : Lang.Get("scarecrow:tooltip-inventory-unavailable");
                text.AppendLine(Lang.Get("scarecrow:tooltip-inventory-" + slot.Code, EscapeTooltipText(name)));
            }
            return text;
        }

        private string GetAnimalGroupName(string prefix)
        {
            string cacheKey = Lang.CurrentLocale + ":" + prefix;
            if (animalGroupNames.TryGetValue(cacheKey, out string cached)) return cached;
            AnimalSpawnRule rule = GetAnimalSpawnRule(prefix);
            if (rule.IsQualified)
            {
                string group = rule.GroupHint;
                string translated = group.Length == 0 ? Lang.Get("scarecrow:tooltip-animal-all")
                    : Lang.GetMatchingIfExists("scarecrow:tooltip-animal-" + group);
                if (translated == null && rule.Domain != "*")
                    translated = Lang.GetMatchingIfExists(rule.Domain + ":item-creature-" + group);
                if (translated == null)
                {
                    // Pick one translated representative instead of listing every age/color
                    // variant. Respect the same domain/path filter as actual spawn blocking.
                    translated = World.EntityTypes.Where(type => rule.Matches(type.Code))
                        .OrderBy(type => type.Code.ToString(), StringComparer.Ordinal)
                        .Select(type => Lang.GetMatchingIfExists(type.Code.Domain + ":item-creature-" + group)
                            ?? Lang.GetMatchingIfExists(type.Code.Domain + ":item-creature-" + type.Code.Path))
                        .FirstOrDefault(value => value != null);
                }
                string qualified = EscapeTooltipText(translated ?? group) + " (" + EscapeTooltipText(rule.Domain) + ")";
                animalGroupNames[cacheKey] = qualified;
                return qualified;
            }
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
        // Server spawn hooks distinguish natural spawning from cages and newborn animals.
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

        // Pickup is server-authoritative; preserve gear only after ownership and claim checks.
        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            // A denied interaction must never reach attachment handling in the base class.
            bool pickup = mode == EnumInteractMode.Interact && byEntity.Controls.CtrlKey
                && byEntity.RightHandItemSlot?.Empty == true;
            if (mode == EnumInteractMode.Interact && !(pickup
                ? TryPickup((byEntity as EntityPlayer)?.Player)
                : TryManage((byEntity as EntityPlayer)?.Player))) return;

            if (!Alive || World.Side == EnumAppSide.Client || mode != EnumInteractMode.Interact)
            {
                base.OnInteract(byEntity, slot, hitPosition, mode);
                return;
            }

            if (pickup)
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
            if (!Alive) return Array.Empty<WorldInteraction>();
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
                if (!string.IsNullOrEmpty(prefix) && GetAnimalSpawnRule(prefix).Matches(code)) return true;
            }

            return false;
        }

        private AnimalSpawnRule GetAnimalSpawnRule(string entry)
        {
            if (!animalSpawnRules.TryGetValue(entry, out AnimalSpawnRule rule))
                animalSpawnRules[entry] = rule = new AnimalSpawnRule(entry);
            return rule;
        }
    }
}

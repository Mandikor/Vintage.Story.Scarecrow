using System;
using System.Globalization;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Read-only explanation of the current natural-spawn rules, sent only to the caller.</summary>
    internal sealed class ScarecrowDiagnostics
    {
        private readonly ICoreServerAPI api;

        internal ScarecrowDiagnostics(ICoreServerAPI api)
        {
            this.api = api;
            api.ChatCommands.GetOrCreate("scarecrow")
                .BeginSubCommand("diagnose").WithDescription(Lang.Get("scarecrow:diagnose-help"))
                .RequiresPrivilege(Privilege.controlserver).RequiresPlayer()
                .WithArgs(api.ChatCommands.Parsers.OptionalWord("entitycode"))
                .HandleWith(Execute).EndSubCommand();
        }

        private TextCommandResult Execute(TextCommandCallingArgs args)
        {
            var player = args.Caller?.Player as IServerPlayer;
            string language = player?.LanguageCode ?? Lang.DefaultLocale;
            string T(string key, params object[] values) => Lang.GetL(language, "scarecrow:" + key, values);
            TextCommandResult Report(string message)
            {
                api.SendMessage(player, GlobalConstants.InfoLogChatGroup, "\n" + message, EnumChatType.Notification);
                // An empty result prevents the command API from also echoing into the input channel.
                return TextCommandResult.Success();
            }
            var target = player?.CurrentEntitySelection?.Entity;
            if (target == null || player.Entity.Pos.Dimension != target.Pos.Dimension
                || player.Entity.Pos.SquareDistanceTo(target.Pos) > 64)
                return TextCommandResult.Error(T("diagnose-target"));

            string input = args[0]?.ToString();
            EntityScarecrow crow;
            AssetLocation code;
            double distance;
            string origin = null;
            bool existing = string.IsNullOrEmpty(input);
            if (!existing)
            {
                // Require an exact qualified code: wildcard filters belong in the config,
                // whereas this command answers a question about one registered entity type.
                if (target is not EntityScarecrow selected || !selected.Alive)
                    return TextCommandResult.Error(T("diagnose-target"));
                var type = api.World.EntityTypes.FirstOrDefault(type =>
                    string.Equals(type.Code?.ToString(), input, StringComparison.OrdinalIgnoreCase));
                if (type == null) return TextCommandResult.Error(T("diagnose-code"));
                code = type.Code;
                crow = selected;
                distance = crow.Pos.DistanceTo(player.Entity.Pos);
            }
            else
            {
                if (target is not EntityAgent || target is EntityScarecrow || target is EntityPlayer || !target.Alive)
                    return TextCommandResult.Error(T("diagnose-target"));
                code = target.Code;
                origin = target.Attributes?.GetString("origin", null);
                // Prefer an actual blocker, then a matching filter, then the nearest living
                // scarecrow. Consider all loaded candidates so overlapping protection is explained.
                crow = api.World.LoadedEntities.Values.OfType<EntityScarecrow>()
                    .Where(c => c.Alive && c.Pos.Dimension == target.Pos.Dimension)
                    .OrderByDescending(c => c.MatchingSpawnFilter(code) != null && c.Pos.DistanceTo(target.Pos) <= c.ScareRadius)
                    .ThenByDescending(c => c.MatchingSpawnFilter(code) != null)
                    .ThenBy(c => c.Pos.DistanceTo(target.Pos)).ThenBy(c => c.EntityId).FirstOrDefault();
                distance = crow?.Pos.DistanceTo(target.Pos) ?? 0;
            }

            string header = existing ? T("diagnose-origin", Escape(origin ?? T("diagnose-unknown"))) + "\n" : "";
            if (existing && origin != "entityspawner") header += T("diagnose-exempt") + "\n";
            if (crow == null) return Report(header + T("diagnose-no-crow"));
            string filter = crow.MatchingSpawnFilter(code);
            bool blocked = filter != null && distance <= crow.ScareRadius;
            string result = T(blocked ? "diagnose-yes" : "diagnose-no");
            if (!blocked) result += ": " + T(filter == null ? "diagnose-filter-miss" : "diagnose-outside");
            CultureInfo culture;
            try { culture = CultureInfo.GetCultureInfo(language); }
            catch (CultureNotFoundException) { culture = CultureInfo.InvariantCulture; }
            // Creature names use the native translation when available; retain the precise code
            // for diagnosis. No origin, configuration, equipment or entity state is modified.
            string name = Lang.GetL(language, code.Domain + ":item-creature-" + code.Path);
            string entityName = name == code.Domain + ":item-creature-" + code.Path ? code.ToString()
                : name + " (" + code + ")";
            string crowName = Lang.GetL(language, crow.Code.Domain + ":item-creature-" + crow.Code.Path);
            string report = T("diagnose-report", Escape(crowName), Escape(entityName), Escape(filter ?? "—"),
                crow.BaseScareRadius.ToString(culture), (crow.ScareRadius - crow.BaseScareRadius).ToString(culture),
                crow.ScareRadius.ToString(culture), distance.ToString("N1", culture), result);
            return Report(header + report);
        }

        private static string Escape(string value) => value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}

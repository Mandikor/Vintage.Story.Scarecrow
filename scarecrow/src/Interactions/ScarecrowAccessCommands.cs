using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Owner-only, server-side access commands for the currently selected scarecrow.</summary>
    internal sealed class ScarecrowAccessCommands
    {
        private readonly ICoreServerAPI api;
        internal ScarecrowAccessCommands(ICoreServerAPI api)
        {
            this.api = api;
            var parsers = api.ChatCommands.Parsers;
            api.ChatCommands.Create("scarecrow")
                .WithDescription(Lang.Get("scarecrow:access-help")).RequiresPrivilege(Privilege.chat)
                .BeginSubCommand("access").WithDescription(Lang.Get("scarecrow:access-help")).RequiresPlayer()
                    .BeginSubCommand("add").WithDescription(Lang.Get("scarecrow:access-help"))
                        .WithArgs(parsers.Word("player"), parsers.Word("permission"))
                        .HandleWith(args => Execute(args, "add")).EndSubCommand()
                    .BeginSubCommand("remove").WithDescription(Lang.Get("scarecrow:access-help"))
                        .WithArgs(parsers.Word("player"))
                        .HandleWith(args => Execute(args, "remove")).EndSubCommand()
                    .BeginSubCommand("list").WithDescription(Lang.Get("scarecrow:access-help"))
                        .HandleWith(args => Execute(args, "list")).EndSubCommand()
                .EndSubCommand();
        }

        private TextCommandResult Execute(TextCommandCallingArgs args, string action)
        {
            var player = args.Caller?.Player as IServerPlayer;
            string lang = player?.LanguageCode ?? Lang.DefaultLocale;
            string T(string key, params object[] values) => Lang.GetL(lang, "scarecrow:" + key, values);
            TextCommandResult Error(string key) => TextCommandResult.Error(T(key));
            var crow = player?.CurrentEntitySelection?.Entity as EntityScarecrow;
            if (crow == null || !crow.Alive || player.Entity.Pos.SquareDistanceTo(crow.Pos) > 64)
                return Error("access-target");
            if (crow.WatchedAttributes.GetString("ownerUid") != player.PlayerUID)
                return Error("access-owner");
            if (!crow.CanEditSharedAccess(player)) return Error("interaction-error-claim");
            if (action == "list")
            {
                var entries = crow.SharedAccessEntries().Select(entry =>
                {
                    string name = api.World.PlayerByUid(entry.Uid)?.PlayerName
                        ?? api.PlayerData.GetPlayerDataByUid(entry.Uid)?.LastKnownPlayername ?? entry.Name;
                    return Escape(name) + ": " + T("access-" + entry.Permission);
                }).OrderBy(text => text, StringComparer.CurrentCultureIgnoreCase).ToArray();
                return TextCommandResult.Success(entries.Length == 0 ? T("access-empty")
                    : T("access-list", string.Join("\n", entries)));
            }

            string nameArg = args[0]?.ToString();
            string permission = action == "add" ? args[1]?.ToString()?.ToLowerInvariant() : null;
            if (action == "add" && permission != "equipment" && permission != "full")
                return Error("access-permission");
            var online = api.World.AllOnlinePlayers.FirstOrDefault(p =>
                p.PlayerName.Equals(nameArg, StringComparison.OrdinalIgnoreCase));
            var known = online == null ? api.PlayerData.PlayerDataByUid.Values.Where(p =>
                string.Equals(p.LastKnownPlayername, nameArg, StringComparison.OrdinalIgnoreCase)).ToArray() : null;
            if (online == null && (known == null || known.Length != 1)) return Error("access-player");
            string uid = online?.PlayerUID ?? known[0].PlayerUID;
            string name = online?.PlayerName ?? known[0].LastKnownPlayername;
            if (uid == player.PlayerUID) return Error("access-self");
            if (action == "remove")
            {
                if (!crow.RemoveSharedAccess(player, uid)) return Error("access-missing");
                return TextCommandResult.Success(T("access-removed", Escape(name)));
            }
            if (!crow.SetSharedAccess(player, uid, name, permission)) return Error("access-owner");
            return TextCommandResult.Success(T("access-added", Escape(name), T("access-" + permission)));
        }

        private static string Escape(string text) => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}


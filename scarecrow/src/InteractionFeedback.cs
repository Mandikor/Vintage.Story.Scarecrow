using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Scarecrow
{
    internal static class InteractionFeedback
    {
        private sealed class RecentFailure
        {
            public string Reason;
            public long LastAttempt;
        }

        private static readonly ConditionalWeakTable<IPlayer, RecentFailure> RecentFailures = new();

        public static void Show(IPlayer player, string reason)
        {
            // Only the server sends feedback, avoiding duplicate client prediction messages.
            if (player is not IServerPlayer serverPlayer || player.Entity?.Api is not ICoreServerAPI api) return;
            long now = api.World.ElapsedMilliseconds;
            RecentFailure recent = RecentFailures.GetOrCreateValue(player);
            bool repeat = recent.Reason == reason && now - recent.LastAttempt < 2500;
            recent.Reason = reason;
            recent.LastAttempt = now;
            if (repeat) return;

            api.SendIngameError(serverPlayer, "scarecrow-" + reason,
                Lang.GetL(serverPlayer.LanguageCode ?? Lang.DefaultLocale, "scarecrow:interaction-error-" + reason));
        }
    }
}

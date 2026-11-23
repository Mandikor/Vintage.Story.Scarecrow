using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Scarecrow
{
    public partial class EntityScarecrow
    {
        // Watched attributes are saved with the entity and synchronized so client
        // interaction prediction uses the same grants as authoritative server checks.
        internal const string SharedAccessAttribute = "scarecrowSharedAccess";

        internal bool HasSharedAccess(string uid, bool pickup)
        {
            if (string.IsNullOrEmpty(uid)) return false;
            string permission = WatchedAttributes.GetTreeAttribute(SharedAccessAttribute)?
                .GetTreeAttribute(uid)?.GetString("permission");
            return permission == "full" || (!pickup && permission == "equipment");
        }

        internal bool CanEditSharedAccess(IPlayer player) => Alive && World.Side == EnumAppSide.Server
            && player != null && !string.IsNullOrEmpty(player.PlayerUID)
            && WatchedAttributes.GetString("ownerUid") == player.PlayerUID
            && World.Claims.TryAccess(player, Pos.AsBlockPos, EnumBlockAccessFlags.Use);

        internal bool SetSharedAccess(IPlayer owner, string uid, string name, string permission)
        {
            if (!CanEditSharedAccess(owner) || string.IsNullOrEmpty(uid) || uid == owner.PlayerUID
                || (permission != "equipment" && permission != "full")) return false;
            var grants = WatchedAttributes.GetOrAddTreeAttribute(SharedAccessAttribute);
            var grant = grants.GetOrAddTreeAttribute(uid);
            grant.SetString("permission", permission);
            grant.SetString("name", name ?? uid);
            SharedAccessChanged();
            return true;
        }

        internal bool RemoveSharedAccess(IPlayer owner, string uid)
        {
            if (!CanEditSharedAccess(owner) || string.IsNullOrEmpty(uid)) return false;
            var grants = WatchedAttributes.GetTreeAttribute(SharedAccessAttribute);
            if (grants == null || !grants.HasAttribute(uid)) return false;
            grants.RemoveAttribute(uid);
            if (grants.Count == 0) WatchedAttributes.RemoveAttribute(SharedAccessAttribute);
            SharedAccessChanged();
            return true;
        }

        internal IEnumerable<(string Uid, string Name, string Permission)> SharedAccessEntries()
        {
            var grants = WatchedAttributes.GetTreeAttribute(SharedAccessAttribute);
            if (grants == null) yield break;
            foreach (var entry in grants)
            {
                if (entry.Value is not ITreeAttribute grant) continue;
                string permission = grant.GetString("permission");
                if (permission == "equipment" || permission == "full")
                    yield return (entry.Key, grant.GetString("name", entry.Key), permission);
            }
        }

        private void SharedAccessChanged()
        {
            WatchedAttributes.MarkPathDirty(SharedAccessAttribute);
            World.BlockAccessor?.GetChunkAtBlockPos(Pos.AsBlockPos)?.MarkModified();
        }
    }
}

using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace Scarecrow
{
    /// <summary>Legacy path prefixes or domain-qualified exact/glob filters, parsed once per entity.</summary>
    internal sealed class AnimalSpawnRule
    {
        public readonly string Domain;
        public readonly string Path;
        public readonly bool IsValid;
        public bool IsQualified => Domain != null;

        public AnimalSpawnRule(string entry)
        {
            if (string.IsNullOrEmpty(entry)) return;
            int separator = entry.IndexOf(':');
            if (separator < 0) { Path = entry; IsValid = true; return; }
            Domain = entry[..separator].Trim().ToLowerInvariant();
            Path = entry[(separator + 1)..].Trim();
            // Only the entire domain may be '*'. Do not expose the engine's regex
            // extension here: malformed user patterns must never throw during spawning.
            IsValid = Domain.Length > 0 && Path.Length > 0 && !Path.Contains(':')
                && !Path.StartsWith('@') && (Domain == "*" || !Domain.Contains('*'));
        }

        public bool Matches(AssetLocation code)
        {
            if (!IsValid || code?.Path == null) return false;
            if (!IsQualified) return code.Path.StartsWith(Path, StringComparison.OrdinalIgnoreCase);
            return (Domain == "*" || Domain.Equals(code.Domain, StringComparison.OrdinalIgnoreCase))
                && WildcardUtil.Match(Path, code.Path);
        }

        // Entity naming conventions vary by mod. Prefer the first literal group
        // segment as a compact fallback; translated entity names are resolved separately.
        public string GroupHint => Path?.Split('-').Select(part => part.Replace("*", "").Trim())
            .FirstOrDefault(part => part.Length > 0)?.ToLowerInvariant() ?? "";
    }
}

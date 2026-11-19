using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Scarecrow
{
    /// <summary>
    /// Adapts resolved wild Equus: Expanded types after its builder has changed combat and flee settings.
    /// JSON patches run before that builder, so inherited tasks alone cannot cover breed-specific changes.
    /// </summary>
    internal static class EquusPoniesCompatibility
    {
        private static readonly string[] Targets = { "scarecrow", "little-scarecrow", "strawdummy" };

        public static void Apply(ICoreAPI api)
        {
            // AI configuration is server-only. Absent optional mods cause no entity scans or allocations.
            if (api.Side != EnumAppSide.Server || !api.ModLoader.IsModEnabled("equusponies")) return;
            foreach (var type in api.World.EntityTypes)
            {
                if (type?.Code?.Domain != "equusferus"
                    || !type.Code.Path.StartsWith("wild", StringComparison.Ordinal)
                    || type.Server?.BehaviorsAsJsonObj == null) continue;

                // Replace a copy instead of editing shared/read-only behavior JSON in place.
                var behaviors = (JsonObject[])type.Server.BehaviorsAsJsonObj.Clone();
                for (int i = 0; i < behaviors.Length; i++)
                {
                    if (behaviors[i]?["code"].AsString() != "taskai"
                        || behaviors[i]["aitasks"].Token is not JArray tasks) continue;
                    var config = (JObject)behaviors[i].Token.DeepClone();
                    config["aitasks"] = AdaptTasks(tasks);
                    behaviors[i] = new JsonObject(config);
                }
                type.Server.BehaviorsAsJsonObj = behaviors;
            }
        }

        internal static JArray AdaptTasks(JArray tasks)
        {
            var result = new JArray();
            var added = new JArray();
            foreach (JObject source in tasks)
            {
                var task = (JObject)source.DeepClone();
                var property = task.Property("entityCodes", StringComparison.OrdinalIgnoreCase);
                if (property?.Value is JArray codes)
                {
                    // Rebuild inherited scarecrow tasks from the final native player tasks. Guard breeds
                    // alter damage and remove player flee targets after the template was patched.
                    var kept = codes.Values<string>().Where(code => !IsScarecrow(code)).ToArray();
                    if (kept.Length == 0 && codes.Count > 0) continue;
                    if (kept.Length != codes.Count) property.Value = JArray.FromObject(kept);
                    if (kept.Contains("player")
                        && task.GetValue("enabled", StringComparison.OrdinalIgnoreCase)?.Value<bool>() != false)
                    {
                        var copy = (JObject)task.DeepClone();
                        copy.Property("entityCodes", StringComparison.OrdinalIgnoreCase).Value = JArray.FromObject(Targets);
                        copy.Property("entityTags", StringComparison.OrdinalIgnoreCase)?.Remove();
                        Set(copy, "retaliateAttacks", false);
                        if (copy.Property("instafleeOnDamageChance", StringComparison.OrdinalIgnoreCase) != null)
                            Set(copy, "instafleeOnDamageChance", 0);
                        added.Add(copy);
                    }
                }
                result.Add(task);
            }
            foreach (var task in added) result.Add(task);
            return result;
        }

        private static bool IsScarecrow(string code) => Targets.Contains(code)
            || code is "scarecrow:scarecrow" or "scarecrow:little-scarecrow" or "game:strawdummy";

        private static void Set(JObject task, string key, JToken value)
        {
            task.Property(key, StringComparison.OrdinalIgnoreCase)?.Remove();
            task[key] = value;
        }
    }
}

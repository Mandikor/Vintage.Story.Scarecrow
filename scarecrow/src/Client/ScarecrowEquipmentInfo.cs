using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Client-only equipment descriptions derived from the resolved inventory whitelists.</summary>
    internal sealed class ScarecrowEquipmentInfo
    {
        internal const string GuidePage = "scarecrow-guide";
        private const string SectionTitle = "scarecrow:equipment-heading";
        private readonly ScarecrowClientConfig config;
        private readonly List<(string Name, string[] Slots)> placements = new();

        private ScarecrowEquipmentInfo(ScarecrowClientConfig config) { this.config = config; }

        internal static void Install(ICoreClientAPI api, ScarecrowClientConfig config)
        {
            // Sided behavior JSON already contains the merged common behaviorConfigs.
            var slotsByModel = new Dictionary<string, JObject[]>();
            foreach (string name in new[] { "scarecrow", "little-scarecrow" })
            {
                var entity = api.World.GetEntityType(new AssetLocation("scarecrow", name));
                var behavior = entity?.Client?.BehaviorsAsJsonObj?.FirstOrDefault(b =>
                    b["code"].AsString() == "scarecrowattachments");
                if (behavior?["wearableSlots"].Token is JArray slots)
                    slotsByModel[name] = slots.OfType<JObject>().ToArray();
            }
            foreach (var collectible in api.World.Collectibles)
            {
                if (collectible?.Code == null) continue;
                bool guideItem = new[] { "scarecrow:scarecrow", "scarecrow:little-scarecrow", "game:strawdummy" }
                    .Contains(collectible.Code.ToString());
                if (collectible.GetBehavior<EquipmentTooltip>() != null) continue;
                var info = new ScarecrowEquipmentInfo(config);
                var stack = new ItemStack(collectible);
                foreach (var model in slotsByModel)
                {
                    string[] slots = model.Value.Where(slot => slot["allowedCodes"] is JArray codes
                        && codes.Values<string>().Any(pattern => WildcardUtil.Match(new AssetLocation(pattern), collectible.Code)))
                        .Select(slot => (string)slot["code"]).ToArray();
                    if (slots.Length > 0) info.placements.Add((model.Key, slots));
                }
                // Do not query unrelated mods' attachment interfaces just to build optional UI.
                if (info.placements.Count > 0
                    && IAttachableToEntity.FromCollectible(collectible)?.GetCategoryCode(stack) == null)
                    info.placements.Clear();
                if (info.placements.Count == 0 && !guideItem) continue;
                if (info.placements.Count > 0)
                    collectible.CollectibleBehaviors = (collectible.CollectibleBehaviors ?? Array.Empty<CollectibleBehavior>())
                        .Append(new EquipmentTooltip(collectible, info)).ToArray();

                // Native handbook pages include the held description. With tooltips enabled it
                // already contains these details, so add only the guide link in that case.
                string body = info.placements.Count > 0 && !config.ShowEquipmentTooltips ? info.Body(stack) + "\n\n" : "";
                body += "<a href=\"handbook://" + GuidePage + "\">" + Lang.Get("scarecrow:guide-title") + "</a>";
                var attributes = collectible.Attributes?.Token?.DeepClone() as JObject ?? new JObject();
                if (attributes["handbook"] is not JObject handbook)
                {
                    handbook = new JObject();
                    attributes["handbook"] = handbook;
                }
                if (handbook["extraSections"] is not JArray sections)
                {
                    sections = new JArray();
                    handbook["extraSections"] = sections;
                }
                string title = guideItem ? "scarecrow:guide-title" : SectionTitle;
                if (sections.OfType<JObject>().Any(s => (string)s["title"] == title)) continue;
                sections.Add(new JObject { ["title"] = title, ["textParts"] = new JArray(body) });
                collectible.Attributes = new JsonObject(attributes);
                // Respect existing handbook behaviors, including subclasses from other mods.
                var existing = collectible.CollectibleBehaviors?.OfType<CollectibleBehaviorHandbookTextAndExtraInfo>().FirstOrDefault();
                if (existing != null)
                    existing.ExtraHandBookSections = sections.ToObject<ExtraHandbookSection[]>();
            }
        }

        internal string Body(ItemStack stack)
        {
            var lines = new List<string>();
            foreach (var placement in placements)
            {
                var slots = new List<string>();
                bool left = placement.Slots.Contains("sc-leftarm"), right = placement.Slots.Contains("sc-rightarm");
                if (left || right) slots.Add(Lang.Get("scarecrow:equipment-" + (left && right ? "arms" : left ? "left" : "right")));
                if (placement.Slots.Contains("sc-neck")) slots.Add(Lang.Get("scarecrow:equipment-neck"));
                if (placement.Slots.Contains("sc-foot")) slots.Add(Lang.Get("scarecrow:equipment-foot"));
                lines.Add(Lang.Get("scarecrow:item-" + placement.Name) + ": " + string.Join(", ", slots));
            }
            var text = new StringBuilder(Lang.Get("scarecrow:equipment-placement", string.Join("; ", lines)));
            bool arms = placements.Any(p => p.Slots.Any(s => s == "sc-leftarm" || s == "sc-rightarm"));
            bool radiusSlot = arms || placements.Any(p => p.Slots.Contains("sc-neck"));
            int bonus = radiusSlot ? ScarecrowAttachments.GetEquipmentRadiusBonus(stack) : 0;
            if (bonus > 0)
                text.AppendLine().Append(Lang.Get("scarecrow:equipment-bonus" + (bonus == 1 ? "-one" : ""), bonus))
                    .AppendLine().Append(Lang.Get("scarecrow:equipment-strongest"));
            text.AppendLine().Append(Lang.Get("scarecrow:equipment-wind",
                Lang.Get("scarecrow:equipment-wind-" + (arms && ScarecrowPendulum.IsEnabled(stack) ? "arms" : "none"))));
            return text.ToString();
        }

        private sealed class EquipmentTooltip : CollectibleBehavior
        {
            private readonly ScarecrowEquipmentInfo info;
            internal EquipmentTooltip(CollectibleObject collectible, ScarecrowEquipmentInfo info) : base(collectible) { this.info = info; }
            public override void GetHeldItemInfo(ItemSlot slot, StringBuilder text, IWorldAccessor world, bool withDebugInfo)
            {
                if (!info.config.ShowEquipmentTooltips || slot?.Itemstack == null) return;
                if (text.Length > 0 && text[text.Length - 1] != '\n') text.AppendLine();
                text.AppendLine().AppendLine("<font color=\"#FFC850\"><strong>" + Lang.Get(SectionTitle) + "</strong></font>");
                text.AppendLine(info.Body(slot.Itemstack));
            }
        }
    }
}

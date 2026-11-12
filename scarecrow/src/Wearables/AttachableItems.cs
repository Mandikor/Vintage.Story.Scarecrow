using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Scarecrow
{
    /// <summary>
    /// Decides which collectibles may be attached to the scarecrow slots.
    /// <para>
    /// The list lives in code on purpose. The vanilla "attachableToEntity/categoryCode" attribute is
    /// shared with other mods and entities, so writing our categories into it makes the result depend
    /// on the patch order. Instead every matching collectible gets a
    /// <see cref="ScarecrowAttachableBehavior"/>, which IAttachableToEntity.FromCollectible prefers
    /// over the attribute.
    /// </para>
    /// </summary>
    public static class AttachableItems
    {
        public const string BehaviorCode = "ScarecrowAttachable";

        // Pattern rules, as evaluated by WildcardUtil.Match:
        //   - The domain has to match exactly, unless it is "*".
        //   - "*" is a wildcard: "scythe-*" matches "scythe-steel", but not "scythe".
        //   - A leading "@" turns the path into a regular expression: "@hoe-(copper|steel)".
        //   - The pattern is compared against the resolved variant code, so every material and
        //     colour variant matches on its own. Blocks work the same way as items.
        //   - The first matching entry wins, so put specific patterns above general ones.
        //   - A pattern that matches nothing is not an error, it is simply skipped.
        private static readonly (AssetLocation Pattern, string CategoryCode)[] Entries =
        {
            (new AssetLocation("game", "scythe-*"), "scythe"),
            (new AssetLocation("game", "hoe-*"), "hoe"),
            (new AssetLocation("game", "bugnet-*"), "bugnet"),
            (new AssetLocation("game", "snowshovel-*"), "snowshovel"),
            (new AssetLocation("game", "wateringcan-*-fired"), "wateringcan"),
            (new AssetLocation("artofgrowing", "hayfork-*"), "hayfork"),
            (new AssetLocation("leaflitter", "rake"), "rake"),
        };

        /// <summary>
        /// Runs in AssetsFinalize, i.e. after every mod has applied its json patches.
        /// </summary>
        public static void Apply(ICoreAPI api)
        {
            int count = 0;

            foreach (CollectibleObject collectible in api.World.Collectibles)
            {
                if (collectible?.Code == null) continue;

                foreach ((AssetLocation pattern, string categoryCode) in Entries)
                {
                    if (!WildcardUtil.Match(pattern, collectible.Code)) continue;

                    if (Inject(api, collectible, categoryCode)) count++;
                    break;
                }
            }

            api.Logger.Notification("[Scarecrow] Registered {0} attachable collectible variants.", count);
        }

        private static bool Inject(ICoreAPI api, CollectibleObject collectible, string categoryCode)
        {
            if (collectible.CollectibleBehaviors != null)
            {
                foreach (CollectibleBehavior behavior in collectible.CollectibleBehaviors)
                {
                    if (behavior is ScarecrowAttachableBehavior) return false;
                }
            }

            ScarecrowAttachableBehavior attachable = new(collectible);
            attachable.Initialize(new JsonObject(new JObject { ["categoryCode"] = categoryCode }));
            attachable.OnLoaded(api);

            // Prepend, so this is the first IAttachableToEntity that FromCollectible finds.
            collectible.CollectibleBehaviors = new CollectibleBehavior[] { attachable }
                .Concat(collectible.CollectibleBehaviors ?? new CollectibleBehavior[0])
                .ToArray();

            return true;
        }
    }
}

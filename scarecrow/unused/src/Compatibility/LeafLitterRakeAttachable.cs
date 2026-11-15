using Newtonsoft.Json.Linq;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Scarecrow
{
    /// <summary>
    /// Registers the Leaf Litter rake for the scarecrow arm slots.
    /// <para>
    /// This cannot be done with a json patch: leaflitter's rake.json starts with // comment
    /// lines, and the vanilla patch loader reads its targets with JToken.Parse(). That returns
    /// the leading comment as document root, so any path such as /attributes fails with
    /// "Cannot access child value on Newtonsoft.Json.Linq.JValue".
    /// </para>
    /// <para>
    /// The attribute payload lives in assets/scarecrow/config/leaflitter-rake-attachable.json.
    /// </para>
    /// </summary>
    public static class LeafLitterRakeAttachable
    {
        private static readonly AssetLocation PayloadLocation = new AssetLocation("scarecrow:config/leaflitter-rake-attachable.json");
        private static readonly AssetLocation RakeCode = new AssetLocation("leaflitter:rake");

        /// <summary>
        /// Runs after the item types have been loaded, so the attribute ends up in the item
        /// type packet that the server sends to every joining client.
        /// </summary>
        public static void Apply(ICoreAPI api)
        {
            Item rake = api.World.GetItem(RakeCode);
            if (rake == null) return;

            IAsset payload = api.Assets.TryGet(PayloadLocation);
            if (payload == null)
            {
                api.Logger.Warning("[Scarecrow] Missing {0}. The Leaf Litter rake cannot be attached to the scarecrow.", PayloadLocation);
                return;
            }

            JObject attributes = rake.Attributes?.Token as JObject;
            if (attributes == null)
            {
                attributes = new JObject();
                rake.Attributes = new JsonObject(attributes);
            }

            // Respect attributes that a future leaflitter update or another mod already provides.
            if (attributes.GetValue("attachableToEntity", StringComparison.OrdinalIgnoreCase) != null) return;

            attributes["attachableToEntity"] = JToken.Parse(payload.ToText());
            api.Logger.Notification("[Scarecrow] Registered the Leaf Litter rake for the scarecrow arm slots.");
        }
    }
}

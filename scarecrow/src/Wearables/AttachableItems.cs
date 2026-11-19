using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Fills missing attachment categories after asset patches without replacing vanilla or mod interfaces.</summary>
    public static class AttachableItems
    {
        // Supply missing metadata only; never replace a category or collectible interface.
        public static void Apply(ICoreAPI api)
        {
            foreach (CollectibleObject collectible in api.World.Collectibles)
            {
                SupplyMissingCategory(collectible);
            }
        }

        public static void SupplyMissingCategory(CollectibleObject collectible)
        {
            if (collectible?.Code == null) return;
            JsonObject attributes = collectible.Attributes;
            if (attributes?["attachableToEntity"]["attachedShapeBySlotCode"].Token is not JObject shapes
                || !shapes.Properties().Any(property => property.Name.StartsWith("sc-", StringComparison.Ordinal))) return;
            if (IAttachableToEntity.FromCollectible(collectible)?.GetCategoryCode(new ItemStack(collectible)) != null) return;
            if (collectible.GetCollectibleInterface<IAttachableToEntity>() != null) return;
            ((JObject)attributes["attachableToEntity"].Token)["categoryCode"] = "scarecrowattachment";
        }
    }
}

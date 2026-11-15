using System;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Scarecrow
{
    public class ScarecrowAttachmentSlot : ItemSlotWearable
    {
        private readonly AssetLocation[] allowedCodes;

        public ScarecrowAttachmentSlot(InventoryBase inventory, string[] categories, string[] codes)
            : base(inventory, categories)
        {
            allowedCodes = (codes ?? Array.Empty<string>()).Select(code => new AssetLocation(code)).ToArray();
        }

        public bool Allows(ItemStack stack)
        {
            return stack?.Collectible?.Code != null
                && allowedCodes.Any(pattern => WildcardUtil.Match(pattern, stack.Collectible.Code));
        }

        public override bool CanHold(ItemSlot sourceSlot)
        {
            return Allows(sourceSlot?.Itemstack) && base.CanHold(sourceSlot);
        }

        public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
        {
            return Allows(sourceSlot?.Itemstack) && base.CanTakeFrom(sourceSlot, priority);
        }
    }
}

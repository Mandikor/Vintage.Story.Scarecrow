using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Scarecrow
{
    /// <summary>Transfers pickup items to the hotbar first, then the backpack, respecting normal slot rules.</summary>
    internal static class ScarecrowPickup
    {
        public static bool TryGive(IPlayer player, ItemStack stack)
        {
            DummySlot source = new(stack);
            GiveToInventory(player, source, player.InventoryManager.GetHotbarInventory());
            if (!source.Empty)
                GiveToInventory(player, source, player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName));
            return source.Empty;
        }

        private static void GiveToInventory(IPlayer player, ItemSlot source, IInventory inventory)
        {
            if (inventory == null) return;
            // Fill existing empty-scarecrow stacks before taking a free slot.
            for (int pass = 0; pass < 2 && !source.Empty; pass++)
            {
                foreach (ItemSlot target in inventory)
                {
                    if (target.Empty != (pass == 1)
                        || (target.StorageType & (EnumItemStorageFlags.Backpack | EnumItemStorageFlags.Offhand)) != 0) continue;
                    source.TryPutInto(player.Entity.World, target, source.StackSize);
                    if (source.Empty) break;
                }
            }
        }
    }
}

using HarmonyLib;
using Vintagestory.GameContent;

namespace Scarecrow
{
    [HarmonyPatch(typeof(CollectibleObject), nameof(CollectibleObject.GetHeldItemInfo))]
    public class GetHeldItemInfoPatch
    {
        public static void Postfix(ItemSlot inSlot, StringBuilder dsc)
        {
            var domain = inSlot.Itemstack?.Collectible?.Code?.Domain;
            if (domain != null)
            {
                if (domain == "scarecrow")
                {
                    dsc.AppendLine("\n<font color=\"#84ef8a\"><i>Scarecrow</i></font>");
                }
            }
        }
    }
}

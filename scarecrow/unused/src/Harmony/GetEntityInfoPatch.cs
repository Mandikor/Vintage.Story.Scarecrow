using HarmonyLib;
using Vintagestory.GameContent;

namespace Scarecrow
{
    [HarmonyPatch(typeof(EntityBehavior), nameof(EntityBehavior.GetInfoText))]
    public class GetEntityInfoPatch
    {
        public static IPlayer forPlayer;
        public static void Postfix(ref string __result)
        {
            //StringBuilder stringBuilder = new StringBuilder();

            var domain = forPlayer.Entity?.EntitySelection?.Entity?.Code?.Domain;
            if (domain != null)
            {
                if (domain == "scarecrow")
                {
                    __result += "\n\n<font color=\"#84ef8a\"><i>Scarecrow</i></font>\n";
                }
            }
        }
    }
}

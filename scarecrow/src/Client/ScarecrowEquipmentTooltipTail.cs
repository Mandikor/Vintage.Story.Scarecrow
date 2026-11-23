using System;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;

namespace Scarecrow
{
    /// <summary>Appends equipment information after all normal item-description overrides have returned.</summary>
    internal sealed class ScarecrowEquipmentTooltipTail : IDisposable
    {
        private const string PatchId = "scarecrow.client.equipment-tooltip";
        private readonly Harmony harmony = new Harmony(PatchId);
        private readonly MethodInfo target = AccessTools.DeclaredMethod(typeof(ItemStack), nameof(ItemStack.GetDescription),
            new[] { typeof(IWorldAccessor), typeof(ItemSlot), typeof(bool) });

        internal ScarecrowEquipmentTooltipTail()
        {
            harmony.Patch(target, postfix: new HarmonyMethod(typeof(ScarecrowEquipmentTooltipTail), nameof(Append))
            {
                priority = Priority.Last
            });
        }

        private static void Append(ItemStack __instance, IWorldAccessor world, ItemSlot inSlot, ref string __result)
        {
            // Single-player shares a process with the server. Only the client's own
            // descriptions receive UI text; server searches/serialization remain untouched.
            if (world?.Side == EnumAppSide.Client)
                ScarecrowEquipmentInfo.AppendTooltip(__instance, inSlot, ref __result);
        }

        public void Dispose() => harmony.Unpatch(target, HarmonyPatchType.Postfix, PatchId);
    }
}

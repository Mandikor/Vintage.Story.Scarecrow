using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Shared visual-only pendulum opt-in. Missing metadata preserves legacy behavior.</summary>
    public static class ScarecrowPendulum
    {
        public const string Attribute = "scarecrowPendulum";

        internal static bool IsLegacyHanger(ItemStack stack) => stack?.Block != null
            && (stack.ItemAttributes?["scarecrowArmLantern"].AsBool(false) == true
                || stack.ItemAttributes?["scarecrowHangingDecoration"].AsBool(false) == true);

        internal static bool IsWindChime(ItemStack stack) => stack?.Block != null
            && stack.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true;

        public static bool IsEnabled(ItemStack stack) => stack?.ItemAttributes?[Attribute]
            .AsBool(IsLegacyHanger(stack) || IsWindChime(stack)) ?? false;

        // Default tool frames need a private mesh even at rest: the right model
        // is reflected with corrected normals and triangle winding before upload.
        internal static bool UsesToolFrame(ItemStack stack) => stack?.Collectible != null
            && !IsLegacyHanger(stack) && !IsWindChime(stack)
            && GetAttachmentPoint(stack, 0) == "ScarecrowArmLeftOriginAP"
            && GetAttachmentPoint(stack, 1) == "ScarecrowArmRightOriginAP";

        // Keep hanging textures/particles and reflected tools in their private
        // renderer when motion is disabled; the switch only controls movement.
        internal static bool UsesWholeModelRenderer(ItemStack stack) => IsLegacyHanger(stack)
            || UsesToolFrame(stack)
            || (stack?.Collectible != null && !IsWindChime(stack) && IsEnabled(stack));

        internal static string GetAttachmentPoint(ItemStack stack, int arm)
        {
            if (IsLegacyHanger(stack)) return arm == 0 ? "ScarecrowLanternLeftAP" : "ScarecrowLanternRightAP";
            string slot = arm == 0 ? "sc-leftarm" : "sc-rightarm";
            return stack?.ItemAttributes?["scarecrowPendulumAttachmentPoints"][slot]
                .AsString(arm == 0 ? "ScarecrowArmLeftOriginAP" : "ScarecrowArmRightOriginAP");
        }

        internal static Vec3f GetPivot(ItemStack stack)
        {
            float[] values = stack?.ItemAttributes?["scarecrowPendulumPivot"].AsArray<float>();
            if (values?.Length == 3 && float.IsFinite(values[0]) && float.IsFinite(values[1]) && float.IsFinite(values[2]))
                return new Vec3f(values[0], values[1], values[2]);
            return IsLegacyHanger(stack) ? new Vec3f(.5f, 1, .5f) : new Vec3f();
        }
    }
}

using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Scarecrow
{
    /// <summary>Optional-mod lifecycle hooks. Asset-only integrations need no C# registration.</summary>
    internal static class ModCompatibility
    {
        // Entity types are fully generated and by-type values resolved by AssetsFinalize.
        public static void FinalizeEntities(ICoreAPI api) => EquusPoniesCompatibility.Apply(api);

        public static void RegisterClientRenderers(ICoreClientAPI api)
        {
            // Register only when the mod is installed; its JSON patch selects this renderer.
            if (api.ModLoader.IsModEnabled("windchimes"))
                api.RegisterEntityRendererClass("ScarecrowWindChimeShape", typeof(WindChimeScarecrowRenderer));
        }
    }
}

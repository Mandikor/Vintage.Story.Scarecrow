using Vintagestory.API.Client;

namespace Scarecrow
{
    /// <summary>Client startup hooks for optional mods. Asset-only integrations need no C# registration.</summary>
    internal static class ModCompatibility
    {
        public static void RegisterClientRenderers(ICoreClientAPI api)
        {
            // Register only when the mod is installed; its JSON patch selects this renderer.
            if (api.ModLoader.IsModEnabled("windchimes"))
                api.RegisterEntityRendererClass("ScarecrowWindChimeShape", typeof(WindChimeScarecrowRenderer));
        }
    }
}

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Optional-mod lifecycle hooks. Asset-only integrations need no C# registration.</summary>
    internal static class ModCompatibility
    {
        public static System.IDisposable StartServer(ICoreServerAPI api) => AnimalCagesCompatibility.TryInstall(api);

        // Entity types are fully generated and by-type values resolved by AssetsFinalize.
        public static void FinalizeEntities(ICoreAPI api) => EquusPoniesCompatibility.Apply(api);

        public static void RegisterClientRenderers(ICoreClientAPI api)
        {
            // Vanilla arm lanterns also need this renderer when no optional decoration mod is installed.
            api.RegisterEntityRendererClass("ScarecrowAttachmentShape", typeof(WindChimeScarecrowRenderer));
            // Keep the existing JSON-facing alias for Wind Chimes and saved entity properties.
            api.RegisterEntityRendererClass("ScarecrowWindChimeShape", typeof(WindChimeScarecrowRenderer));
        }
    }
}

using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Registers shared types, loads settings and delegates optional-mod startup hooks.</summary>
    public class Core : ModSystem
    {
        public ScarecrowConfig Config { get; private set; }
        private System.IDisposable serverCompatibility;
        private System.IDisposable transformEditor;

        // Register stable JSON-facing names on both sides before entities or inventories load.
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            LoadConfig(api);

            api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
            api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));

            api.RegisterEntityBehaviorClass("scarecrowattachments", typeof(ScarecrowAttachments));

            api.RegisterEntity("EntityScarecrow", typeof(EntityScarecrow));
        }

        private void LoadConfig(ICoreAPI api)
        {
            Config = ScarecrowConfigLoader.Load(api);
        }

        // Collectible metadata is available after patches, so categories can be inspected safely.
        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);

            AttachableItems.Apply(api);
            ModCompatibility.FinalizeEntities(api);
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            ModCompatibility.RegisterClientRenderers(api);
            transformEditor = new ScarecrowTransformEditor(api);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            serverCompatibility = ModCompatibility.StartServer(api);
        }

        public override void Dispose()
        {
            serverCompatibility?.Dispose();
            serverCompatibility = null;
            transformEditor?.Dispose();
            transformEditor = null;
            base.Dispose();
        }
    }
}

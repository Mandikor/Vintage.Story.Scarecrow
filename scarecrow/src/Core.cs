using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Scarecrow
{
    /// <summary>Registers shared types, loads settings and delegates optional-mod startup hooks.</summary>
    public class Core : ModSystem
    {
        public ScarecrowConfig Config { get; private set; }

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
        }
    }
}

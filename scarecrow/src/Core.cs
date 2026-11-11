using System;
using Vintagestory.API.Common;

namespace Scarecrow
{
    public class Core : ModSystem
    {
        public ScarecrowConfig Config { get; private set; }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            LoadConfig(api);

            api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
            api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));

            api.RegisterEntity("EntityScarecrow", typeof(EntityScarecrow));
        }

        private void LoadConfig(ICoreAPI api)
        {
            try
            {
                Config = api.LoadModConfig<ScarecrowConfig>("scarecrow.json");
            }
            catch (Exception ex)
            {
                // Do not overwrite the file: the user probably just made a typo.
                api.Logger.Error("[Scarecrow] Could not read ModConfig/scarecrow.json, using defaults. Fix the file and restart. Error: {0}", ex.Message);
                Config = new ScarecrowConfig();
            }

            if (Config == null)
            {
                Config = new ScarecrowConfig();
                api.StoreModConfig(Config, "scarecrow.json");
                api.Logger.Notification("[Scarecrow] Wrote default config to ModConfig/scarecrow.json");
            }
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);

            LeafLitterRakeAttachable.Apply(api);
        }
    }
}
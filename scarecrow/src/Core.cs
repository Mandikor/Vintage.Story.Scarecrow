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

            api.RegisterEntityBehaviorClass("scarecrowattachments", typeof(ScarecrowAttachments));

            api.RegisterEntity("EntityScarecrow", typeof(EntityScarecrow));
        }

        private void LoadConfig(ICoreAPI api)
        {
            Config = ScarecrowConfigLoader.Load(api);
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);

            AttachableItems.Apply(api);
        }

        public override void StartClientSide(Vintagestory.API.Client.ICoreClientAPI api)
        {
            base.StartClientSide(api);
            if (api.ModLoader.IsModEnabled("windchimes"))
            {
                api.RegisterEntityRendererClass("ScarecrowWindChimeShape", typeof(WindChimeScarecrowRenderer));
            }
        }
    }
}

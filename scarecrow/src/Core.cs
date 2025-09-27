using Vintagestory.API.Common;

namespace Scarecrow;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
        api.RegisterEntity("EntityLittleScareCrow", typeof(EntityLittleScareCrow));

        api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));
        api.RegisterEntity("EntityScareCrow", typeof(EntityScareCrow));

        api.RegisterEntity("EntitySC_StrawDummy", typeof(EntitySC_StrawDummy));
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);
    }
}

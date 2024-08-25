using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Scarecrow.Configuration;
using Microsoft.VisualBasic;
using Vintagestory.GameContent;

namespace Scarecrow;

public class Core : ModSystem
{

    public Config Config { get; set; }

    public override void StartPre(ICoreAPI api)
    {
        //if (api.Side.IsServer())
        //{
            Config = ModConfig.ReadConfig<Config>(api, "MandikorsMods/ScareCrowConfig.json");
            #region 
            api.World.Config.SetBool("Scarecrow_Scarecrow_Enabled", Config.EnabledScarecrow);
            api.World.Config.SetBool("Scarecrow_LittleScarecrow_Enabled", Config.EnabledLittleScarecrow);
            api.World.Config.SetBool("Scarecrow_Strawdummy_Enabled", Config.EnabledStrawdummy);

            api.World.Config.SetInt("Scarecrow_Blockingradius_Scarecrow", Config.BlockRadiusScarecrow);
            api.World.Config.SetInt("Scarecrow_Blockingradius_LittleScarecrow", Config.BlockRadiusLittleScarecrow);
            api.World.Config.SetInt("Scarecrow_Blockingradius_Strawdummy", Config.BlockRadiusStrawdummy);
            #endregion
        //}
        //if (api.Side.IsClient())
        //{
        //    Config = ModConfig.ReadConfig<Config>(api, "MandikorsMods/ScareCrowConfig.json");
        //}
    }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
        api.RegisterEntity("EntityLittleScareCrow", typeof(EntityLittleScareCrow));

        api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));
        api.RegisterEntity("EntityScareCrow", typeof(EntityScareCrow));

        api.RegisterEntity("EntitySC_StrawDummy", typeof(EntitySC_StrawDummy));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);
    }
}

﻿global using HarmonyLib;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using Vintagestory.API.Client;
global using Vintagestory.API.Common;
global using Vintagestory.API.Common.Entities;
global using Vintagestory.API.Config;
global using Vintagestory.API.Datastructures;
global using Vintagestory.API.MathTools;
global using Vintagestory.API.Util;
global using Vintagestory.Client.NoObf;
global using Vintagestory.GameContent;
global using Vintagestory.API.Server;

namespace Scarecrow
{
    public class Core : ModSystem
    {
        ICoreAPI api;

        public Config SCConfig
        {
            get
            {
                return (Config)api.ObjectCache["MandikorsMods/ScareCrowConfig.json"];
            }
            set
            {
                api.ObjectCache.Add("MandikorsMods/ScareCrowConfig.json", value);
            }
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.api = api;

            api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
            api.RegisterEntity("EntityLittleScareCrow", typeof(EntityLittleScareCrow));

            api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));
            api.RegisterEntity("EntityScareCrow", typeof(EntityScareCrow));

            api.RegisterEntity("EntitySC_StrawDummy", typeof(EntitySC_StrawDummy));
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            Config scareConfig = null;

            try
            {
                scareConfig = api.LoadModConfig<Config>("MandikorsMods/ScareCrowConfig.json");
            }
            catch (Exception)
            {
                api.Logger.Warning("Scarecrow: Config Exception! Config will be rebuilt.");
            }

            if (scareConfig == null)
            {
                api.Logger.Warning("Scarecrow: Config Error! A typo or a new config setting can cause this. Config will be rebuilt.");
                Config scc = new();
                api.StoreModConfig<Config>(scc, "MandikorsMods/ScareCrowConfig.json");
                scareConfig = api.LoadModConfig<Config>("MandikorsMods/ScareCrowConfig.json");
            }
            SCConfig = scareConfig;
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);
        }
    }
}

namespace Scarecrow
{
    public static class ModConfig
    {
        private const string jsonConfig = "MandikorsMods/ScareCrowConfig.json";

        public static Config ReadConfig(ICoreAPI api)
        {
            Config config;

            try
            {
                config = LoadConfig(api);

                if (config == null)
                {
                    GenerateConfig(api);
                    config = LoadConfig(api);
                }
                else
                {
                    GenerateConfig(api, config);
                }
            }
            catch
            {
                GenerateConfig(api);
                config = LoadConfig(api);
            }

            #region 
            api.World.Config.SetBool("Scarecrow_Scarecrow_Enabled", config.EnabledScarecrow);
            api.World.Config.SetBool("Scarecrow_LittleScarecrow_Enabled", config.EnabledLittleScarecrow);

            api.World.Config.SetInt("Scarecrow_Blockingradius_Scarecrow", config.BlockRadiusScarecrow);
            api.World.Config.SetInt("Scarecrow_Blockingradius_LittleScarecrow", config.BlockRadiusLittleScarecrow);
            api.World.Config.SetInt("Scarecrow_Blockingradius_Strawdummy", config.BlockRadiusStrawdummy);
            #endregion

            return config;

        }
        private static Config LoadConfig(ICoreAPI api)
        {
            return api.LoadModConfig<Config>(jsonConfig);
        }

        private static void GenerateConfig(ICoreAPI api)
        {
            api.StoreModConfig(new Config(), jsonConfig);
        }

        private static void GenerateConfig(ICoreAPI api, Config previousConfig)
        {
            api.StoreModConfig(new Config(previousConfig), jsonConfig);
        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Scarecrow.Configuration;

//private const string jsonConfig = "MandikorsMods/ScareCrowConfig.json";

public static class ModConfig
{
    public static T ReadConfig<T>(ICoreAPI api, string jsonConfig) where T : class, IModConfig
    {
        T config;

        try
        {
            config = LoadConfig<T>(api, jsonConfig);

            if (config == null)
            {
                GenerateConfig<T>(api, jsonConfig);
                config = LoadConfig<T>(api, jsonConfig);
            }
            else
            {
                GenerateConfig(api, jsonConfig, config);
            }
        }
        catch
        {
            GenerateConfig<T>(api, jsonConfig);
            config = LoadConfig<T>(api, jsonConfig);
        }

        return config;
    }

    public static void WriteConfig<T>(ICoreAPI api, string jsonConfig, T config) where T : class, IModConfig
    {
        GenerateConfig(api, jsonConfig, config);
    }

    private static T LoadConfig<T>(ICoreAPI api, string jsonConfig) where T : IModConfig
    {
        return api.LoadModConfig<T>(jsonConfig);
    }

    private static void GenerateConfig<T>(ICoreAPI api, string jsonConfig, T previousConfig = null) where T : class, IModConfig
    {
        api.StoreModConfig(CloneConfig<T>(api, previousConfig), jsonConfig);
    }

    private static T CloneConfig<T>(ICoreAPI api, T config = null) where T : class, IModConfig
    {
        return (T)Activator.CreateInstance(typeof(T), new object[] { api, config });
    }
}

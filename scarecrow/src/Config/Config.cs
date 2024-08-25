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

public class Config : IModConfig
{
    public int BlockRadiusScarecrow { get; set; } = 16;
    public int BlockRadiusLittleScarecrow { get; set; } = 8;
    public int BlockRadiusStrawdummy { get; set; } = 4;

    public bool DebugOutput { get; set; } = false;


    public Config(ICoreAPI api, Config previousConfig = null)
    {
        if (previousConfig == null)
        {
            return;
        }

        BlockRadiusScarecrow = previousConfig.BlockRadiusScarecrow;
        BlockRadiusLittleScarecrow = previousConfig.BlockRadiusLittleScarecrow;
        BlockRadiusStrawdummy = previousConfig.BlockRadiusStrawdummy;

        DebugOutput = previousConfig.DebugOutput;
    }
}

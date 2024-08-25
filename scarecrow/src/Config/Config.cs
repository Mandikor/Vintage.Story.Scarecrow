using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Scarecrow;

public class Config
{
    public bool EnabledScarecrow = true;
    public bool EnabledLittleScarecrow = true;
    public bool EnabledStrawdummy = true;

    public int BlockRadiusScarecrow = 16;
    public int BlockRadiusLittleScarecrow = 8;
    public int BlockRadiusStrawdummy = 4;

    public bool DebugOutput = false;

    public Config() { }

    public Config(Config previousConfig)
    {
        EnabledScarecrow = previousConfig.EnabledScarecrow;
        EnabledLittleScarecrow = previousConfig.EnabledLittleScarecrow;
        EnabledStrawdummy = previousConfig.EnabledStrawdummy;

        BlockRadiusScarecrow = previousConfig.BlockRadiusScarecrow;
        BlockRadiusLittleScarecrow = previousConfig.BlockRadiusLittleScarecrow;
        BlockRadiusStrawdummy = previousConfig.BlockRadiusStrawdummy;

        DebugOutput = previousConfig.DebugOutput;
    }
}

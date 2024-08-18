namespace Scarecrow
{
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
}

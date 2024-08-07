namespace Scarecrow
{
    public class Config
    {
        public bool EnabledScarecrow = true;
        public bool EnabledLittleScarecrow = true;

        public int BlockRadiusScarecrow = 16;
        public int BlockRadiusLittleScarecrow = 8;
        public int BlockRadiusStrawdummy = 4;

        public bool BlockRadiusAsSphere = true;

        public bool DebugOutput = true;
        
        public Config() { }

        public Config(Config previousConfig)
        {
            EnabledScarecrow = previousConfig.EnabledScarecrow;
            EnabledLittleScarecrow = previousConfig.EnabledLittleScarecrow;

            BlockRadiusScarecrow = previousConfig.BlockRadiusScarecrow;
            BlockRadiusLittleScarecrow = previousConfig.BlockRadiusLittleScarecrow;
            BlockRadiusStrawdummy = previousConfig.BlockRadiusStrawdummy;

            BlockRadiusAsSphere = previousConfig.BlockRadiusAsSphere;

            DebugOutput = previousConfig.DebugOutput;
        }
    }
}

namespace Scarecrow
{
    /// <summary>Personal display preferences; never loaded or synchronized by the server.</summary>
    public class ScarecrowClientConfig
    {
        public const int CurrentConfigVersion = 1;
        public int ConfigVersion { get; set; } = CurrentConfigVersion;
        public bool ShowEquipmentTooltips { get; set; } = false;
    }
}

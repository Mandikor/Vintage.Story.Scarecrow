namespace Scarecrow
{
    /// <summary>
    /// Settings stored in ModConfig/scarecrow.json.
    /// </summary>
    public class ScarecrowConfig
    {
        public const int CurrentConfigVersion = 2;

        /// <summary>Configuration schema version, independent of the mod release version.</summary>
        public int ConfigVersion { get; set; } = CurrentConfigVersion;

        /// <summary>Write spawn interception details to the server log.</summary>
        public bool DebugOutput { get; set; } = false;

        /// <summary>
        /// Natural-spawn filters. Plain entries match path prefixes across all domains;
        /// qualified entries match exact domain:path codes or '*' wildcard patterns.
        /// Examples: "hare", "game:hare-*", "modid:animal-adult", "*:hare-*".
        /// Animals born nearby or placed by players are not affected.
        /// </summary>
        public string[] ScaredAnimalPrefixes { get; set; } = { "hare", "raccoon" };
    }
}

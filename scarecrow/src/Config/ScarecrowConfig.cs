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
        /// Entity code prefixes that scarecrows keep away from natural spawns. The prefix
        /// is matched against the entity path, so "hare" covers hare-adult and hare-baby.
        /// Animals born nearby or placed by players are not affected.
        /// </summary>
        public string[] ScaredAnimalPrefixes { get; set; } = { "hare", "raccoon" };
    }
}

namespace Scarecrow
{
    /// <summary>
    /// Settings stored in ModConfig/scarecrow.json.
    /// </summary>
    public class ScarecrowConfig
    {
        /// <summary>Write spawn interception details to the server log.</summary>
        public bool DebugOutput { get; set; } = false;

        /// <summary>
        /// Entity code prefixes that scarecrows keep away. The prefix is matched against
        /// the entity path, so "hare" covers hare-adult and hare-baby.
        /// </summary>
        public string[] ScaredAnimalPrefixes { get; set; } = { "hare", "raccoon" };
    }
}

namespace CCExtractorTester
{
    /// <summary>
    /// Represents the possible comparisons that the program supports.
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// Regular diff (Linux only), using the diff command.
        /// </summary>
        diff,
        /// <summary>
        /// Generates HTML viewable diffs for two files (showing the full file).
        /// </summary>
        diffplex,
        /// <summary>
        /// Generates HTML viewable diffs for two files (showing changes only).
        /// </summary>
        diffplexreduced
    }
}
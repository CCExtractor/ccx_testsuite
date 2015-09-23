using System;

namespace CCExtractorTester.Enums
{
    /// <summary>
    /// Represents the possible comparisons that the program supports.
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// Regular diff (Linux only), using the diff command.
        /// </summary>
        Diff,
        /// <summary>
        /// Generates HTML viewable diffs for two files (showing the full file).
        /// </summary>
        Diffplex,
        /// <summary>
        /// Generates HTML viewable diffs for two files (showing changes only).
        /// </summary>
        Diffplexreduced,
        /// <summary>
        /// Uploads the results to the server, where they will be generated dynamically.
        /// </summary>
        Server
    }

    /// <summary>
    /// A class to parse string values for the CompareType enum.
    /// </summary>
    public static class CompareTypeParser
    {
        /// <summary>
        /// Parses a given string into a CompareType enum option.
        /// </summary>
        /// <param name="toParse">The string to parse.</param>
        /// <returns>An instance of the CompareType enum.</returns>
        public static CompareType parseString(string toParse)
        {
            toParse = toParse.ToLower();
            switch (toParse)
            {
                case "diff":
                    return CompareType.Diff;
                case "diffplex":
                    return CompareType.Diffplex;
                case "diffplexreduced":
                    return CompareType.Diffplexreduced;
                case "server":
                    return CompareType.Server;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
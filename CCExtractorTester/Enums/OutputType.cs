using System;

namespace CCExtractorTester.Enums
{
    /// <summary>
    /// Represents the type of output we expect from CCExtractor
    /// </summary>
    public enum OutputType
    {
        File,
        Null,
        Tcp,
        Cea708,
        Multiprogram,
        Stdout
    }

    /// <summary>
    /// A class to parse string values for the OutputType enum.
    /// </summary>
    public static class OutputTypeParser
    {
        /// <summary>
        /// Parses a given string into a OutputType enum option.
        /// </summary>
        /// <param name="toParse">The string to parse.</param>
        /// <returns>An instance of the OutputType enum.</returns>
        public static OutputType parseString(string toParse)
        {
            toParse = toParse.ToLower();
            switch (toParse)
            {
                case "file":
                    return OutputType.File;
                case "null":
                    return OutputType.Null;
                case "tcp":
                    return OutputType.Tcp;
                case "cea708":
                    return OutputType.Cea708;
                case "multiprogram":
                    return OutputType.Multiprogram;
                case "stdout":
                    return OutputType.Stdout;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

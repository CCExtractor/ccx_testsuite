using System;

namespace CCExtractorTester.Enums
{
    /// <summary>
    /// Represents possible input types for a test entry.
    /// </summary>
    public enum InputType
    {
        File,
        Stdin,
        Udp
    }

    /// <summary>
    /// A class to parse string values for the InputType enum.
    /// </summary>
    public static class InputTypeParser
    {
        /// <summary>
        /// Parses a given string into a InputType enum option.
        /// </summary>
        /// <param name="toParse">The string to parse.</param>
        /// <returns>An instance of the InputType enum.</returns>
        public static InputType parseString(string toParse)
        {
            toParse = toParse.ToLower();
            switch (toParse)
            {
                case "stdin":
                    return InputType.Stdin;
                case "udp":
                    return InputType.Udp;
                case "file":
                    return InputType.File;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

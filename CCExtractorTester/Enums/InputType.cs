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

    public static class InputTypeParser
    {
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

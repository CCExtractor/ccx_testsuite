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
        Multiprogram
    }

    public static class OutputTypeParser
    {
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

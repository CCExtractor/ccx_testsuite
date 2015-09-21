using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCExtractorTester.Analyzers
{
    public class CompareFile
    {
        public string CorrectFile { get; private set; }

        public string ExpectedFile { get; private set; }

        public bool IgnoreOutput { get; private set; }

        public CompareFile(string correctFile, string expectedFile, bool ignoreOutput)
        {
            CorrectFile = correctFile;
            ExpectedFile = expectedFile;
            IgnoreOutput = ignoreOutput;
        }
    }
}
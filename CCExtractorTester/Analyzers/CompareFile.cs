using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCExtractorTester.Analyzers
{
    /// <summary>
    /// A class that indicates what should happen to a single generated output for a single test entry file.
    /// </summary>
    public class CompareFile
    {
        /// <summary>
        /// The correct file in the stored folder.
        /// </summary>
        public string CorrectFile { get; private set; }

        /// <summary>
        /// The filename that CCExtractor will be generating.
        /// </summary>
        public string ExpectedFile { get; private set; }

        /// <summary>
        /// Ignore this generated file and do not compare it?
        /// </summary>
        public bool IgnoreOutput { get; private set; }

        /// <summary>
        /// Creates a new instance for this class.
        /// </summary>
        /// <param name="correctFile">The correct file in the stored folder.</param>
        /// <param name="expectedFile">The filename that CCExtractor will be generating.</param>
        /// <param name="ignoreOutput">Ignore this generated file and do not compare it?</param>
        public CompareFile(string correctFile, string expectedFile, bool ignoreOutput)
        {
            CorrectFile = correctFile;
            ExpectedFile = expectedFile;
            IgnoreOutput = ignoreOutput;
        }
    }
}
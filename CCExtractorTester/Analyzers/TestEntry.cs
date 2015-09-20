using CCExtractorTester.Enums;
using System;
using System.Collections.Generic;

namespace CCExtractorTester.Analyzers
{
	/// <summary>
	/// Test entry that holds the name of the file to test, the command that needs to be used and the name of the result file.
	/// </summary>
	public class TestEntry
	{
        /// <summary>
        /// Gets or sets the input file that will be used.
        /// </summary>
		public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the input type that will be used.
        /// </summary>
        public InputType InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the command that will be passed to CCExtractor
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the output format of the test entry.
        /// </summary>
        public OutputType OutputFormat { get; set; }
        
        /// <summary>
        /// Gets or sets a list of files to compare
        /// </summary>
        public List<Tuple<string, string, bool>> CompareFiles { get; set; }

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="inputFile">The relative location (relative to SampleFolder configuration setting) of the file to test</param>
		/// <param name="command">The command string to pass to CCExtractor.</param>
		/// <param name="compareFile">The relative location (relative to CorrectResultFolder configuration setting) of the result file</param>
		public TestEntry (string inputFile, string command, string compareFile)
		{
			InputFile = inputFile;
            InputFormat = InputType.File;
			Command = command;
            OutputFormat = OutputType.File;
            CompareFiles = new List<Tuple<string, string, bool>>();
            CompareFiles.Add(Tuple.Create(compareFile,compareFile,false));
		}

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="inputFile">The relative location (relative to SampleFolder configuration setting) of the file to test</param>
        /// <param name="inputFormat">The input type for CCExtractor</param>
        /// <param name="command">The command string to pass to CCExtractor.</param>
        /// <param name="outputFormat">The output type for CCExtractor</param>
        /// <param name="compareFiles">The relative locations (relative to CorrectResultFolder configuration setting) of the result files</param>
        public TestEntry(string inputFile, InputType inputFormat, string command, OutputType outputFormat, List<Tuple<string,string,bool>> compareFiles)
        {
            InputFile = inputFile;
            InputFormat = inputFormat;
            Command = command;
            OutputFormat = outputFormat;
            CompareFiles = compareFiles;
        }
	}
}
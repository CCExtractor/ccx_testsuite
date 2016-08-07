using CCExtractorTester.Enums;
using System.Collections.Generic;

namespace CCExtractorTester.Analyzers
{
    /// <summary>
    /// Test entry that holds the name of the file to test, the command that needs to be used and the name of the result file.
    /// </summary>
    public class TestEntry
	{
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

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
        public List<CompareFile> CompareFiles { get; set; }

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="inputFile">The relative location (relative to SampleFolder configuration setting) of the file to test</param>
		/// <param name="command">The command string to pass to CCExtractor.</param>
		/// <param name="compareFile">The relative location (relative to CorrectResultFolder configuration setting) of the result file</param>
		public TestEntry (string inputFile, string command, string compareFile)
		{
            Id = -1;
			InputFile = inputFile;
            InputFormat = InputType.File;
			Command = command;
            OutputFormat = OutputType.File;
            CompareFiles = new List<CompareFile>();
            CompareFiles.Add(new CompareFile(-1, compareFile,compareFile,false));
		}

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="inputFile">The relative location (relative to SampleFolder configuration setting) of the file to test</param>
        /// <param name="inputFormat">The input type for CCExtractor</param>
        /// <param name="command">The command string to pass to CCExtractor.</param>
        /// <param name="outputFormat">The output type for CCExtractor</param>
        /// <param name="compareFiles">The relative locations (relative to CorrectResultFolder configuration setting) of the result files</param>
        public TestEntry(int id, string inputFile, InputType inputFormat, string command, OutputType outputFormat, List<CompareFile> compareFiles)
        {
            Id = id;
            InputFile = inputFile;
            InputFormat = inputFormat;
            Command = command;
            OutputFormat = outputFormat;
            CompareFiles = compareFiles;
        }
    }
}
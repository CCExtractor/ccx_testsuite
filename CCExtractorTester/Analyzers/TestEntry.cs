using System;

namespace CCExtractorTester
{
	/// <summary>
	/// Test entry that holds the name of the file to test, the command that needs to be used and the name of the result file.
	/// </summary>
	public class TestEntry
	{
		public String TestFile { get; private set; }
		public String Command { get; private set; }
		public String ResultFile { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.TestEntry"/> class.
		/// </summary>
		/// <param name="testFile">The relative location (relative to SampleFolder configuration setting) of the file to test</param>
		/// <param name="command">The command string to pass to CCExtractor.</param>
		/// <param name="resultFile">The relative location (relative to CorrectResultFolder configuration setting) of the result file</param>
		public TestEntry (string testFile, string command, string resultFile)
		{
			TestFile = testFile;
			Command = command;
			ResultFile = resultFile;
		}
	}
}
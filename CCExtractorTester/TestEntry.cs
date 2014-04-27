using System;

namespace CCExtractorTester
{
	public class TestEntry
	{
		public String TestFile { get; private set; }
		public String Command { get; private set; }
		public String ResultFile { get; private set; }

		public TestEntry (string testFile, string command, string resultFile)
		{
			TestFile = testFile;
			Command = command;
			ResultFile = resultFile;
		}
	}
}
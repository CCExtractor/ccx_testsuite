using System;

namespace CCExtractorTester
{
	public class CompareData
	{
		public string CorrectFile { get; set; }
		public string ProducedFile { get; set; }
		public TimeSpan RunTime { get; set; }

		public CompareData ()
		{
		}
	}
}
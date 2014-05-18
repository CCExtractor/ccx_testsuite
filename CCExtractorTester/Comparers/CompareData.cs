using System;

namespace CCExtractorTester
{
	public class CompareData
	{
		public string CorrectFile { get; set; }
		public string ProducedFile { get; set; }
		public string SampleFile { get; set; }
		public string Command { get; set; }
		public TimeSpan RunTime { get; set; }
	}
}
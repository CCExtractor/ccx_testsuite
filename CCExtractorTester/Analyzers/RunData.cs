using System;
using System.Collections.Generic;

namespace CCExtractorTester.Analyzers
{
	public class RunData {
		public int ExitCode { get; set; }
		public TimeSpan Runtime { get; set; }

        public Dictionary<string,string> ResultFiles { get; set; }
	}
}
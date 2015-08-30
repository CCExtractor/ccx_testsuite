using System;
using System.Diagnostics;

namespace CCExtractorTester
{
	public class NullPerformanceLogger: IPerformanceLogger {
		public readonly static NullPerformanceLogger Instance = new NullPerformanceLogger();

		private NullPerformanceLogger(){}

		#region IPerformanceLogger implementation

		public void SetUp (ILogger logger, Process p)
		{
			// do nothing
		}

		public void DebugValue ()
		{
			// do nothing
		}

		public void DebugStats ()
		{
			// do nothing
		}
		#endregion
	}
}
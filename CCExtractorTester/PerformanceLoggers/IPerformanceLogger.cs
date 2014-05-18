using System;
using System.Diagnostics;

namespace CCExtractorTester
{
	public interface IPerformanceLogger
	{
		void SetUp(ILogger logger,Process p);
		void DebugValue();
		void DebugStats();
	}
}
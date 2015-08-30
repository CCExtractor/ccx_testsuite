using System.Diagnostics;

namespace CCExtractorTester
{
	/// <summary>
	/// Interface for logging performance.
	/// </summary>
	public interface IPerformanceLogger
	{
		/// <summary>
		/// Sets up the performance logger for a given process and using a given logger.
		/// </summary>
		/// <param name="logger">The logger to use for output.</param>
		/// <param name="p">The process to monitor.</param>
		void SetUp(ILogger logger,Process p);
		/// <summary>
		/// Logs (in debug) the current value.
		/// </summary>
		void DebugValue();
		/// <summary>
		/// Logs (in debug) the stats.
		/// </summary>
		void DebugStats();
	}
}
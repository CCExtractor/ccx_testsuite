using System;
using System.Diagnostics;

namespace CCExtractorTester
{
	/// <summary>
	/// Windows performance counters.
	/// </summary>
	public class WindowsPerformanceCounters : IPerformanceLogger
	{
		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
		private ILogger Logger { get; set; }
		/// <summary>
		/// Gets or sets the performance counter for RAM.
		/// </summary>
		/// <value>The ram.</value>
		private PerformanceCounter Ram { get; set; }
		/// <summary>
		/// Gets or sets the performance counter for the cpu.
		/// </summary>
		/// <value>The cpu.</value>
		private PerformanceCounter Cpu { get; set; }
		/// <summary>
		/// Gets or sets the process that's being monitored.
		/// </summary>
		/// <value>The proc.</value>
		private Process Proc { get; set; }


		#region IPerformanceLogger implementation
		/// <summary>
		/// Sets up the performance logger for a given process and using a given logger.
		/// </summary>
		/// <param name="logger">The logger to use for output.</param>
		/// <param name="p">The process to monitor.</param>
		public void SetUp (ILogger logger,Process p)
		{
			Logger = logger;
			Proc = p;
			Ram = Cpu = null;
			try {
				Ram = new PerformanceCounter ("Process", "Working Set", p.ProcessName);
				Cpu = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
			} catch(Exception e){
				Logger.Error (e);
			}
		}
		/// <summary>
		/// Logs (in debug) the current value.
		/// </summary>
		public void DebugValue ()
		{
			if(Ram != null && Cpu != null){
				try {
				Logger.Debug(String.Format("Process usage stats: {0} MB of ram, {1} % CPU",(Ram.NextValue()/1024/1024),Cpu.NextValue()));
				} catch(Exception){
				}
			}
		}
		/// <summary>
		/// Logs (in debug) the stats.
		/// </summary>
		public void DebugStats ()
		{
			Logger.Debug(String.Format("Process data: handles opened: {0}, peak paged mem: {1}, peak virtual mem: {2}, peak mem: {3}, privileged cpu time: {4}, total cpu time: {5}",Proc.HandleCount,Proc.PeakPagedMemorySize64,Proc.PeakVirtualMemorySize64,Proc.PeakWorkingSet64,Proc.PrivilegedProcessorTime,Proc.TotalProcessorTime));
		}
		#endregion
	}
}
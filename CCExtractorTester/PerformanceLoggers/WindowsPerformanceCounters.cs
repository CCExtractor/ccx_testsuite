using System;
using System.Diagnostics;

namespace CCExtractorTester
{
	public class WindowsPerformanceCounters : IPerformanceLogger
	{
		private ILogger Logger { get; set; }
		private PerformanceCounter Ram { get; set; }
		private PerformanceCounter Cpu { get; set; }
		private Process Proc { get; set; }


		#region IPerformanceLogger implementation

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

		public void DebugValue ()
		{
			if(Ram != null && Cpu != null){
				try {
				Logger.Debug(String.Format("Process usage stats: {0} MB of ram, {1} % CPU",(Ram.NextValue()/1024/1024),Cpu.NextValue()));
				} catch(Exception){
				}
			}
		}


		public void DebugStats ()
		{
			Logger.Debug(String.Format("Process data: handles opened: {0}, peak paged mem: {1}, peak virtual mem: {2}, peak mem: {3}, privileged cpu time: {4}, total cpu time: {5}",Proc.HandleCount,Proc.PeakPagedMemorySize64,Proc.PeakVirtualMemorySize64,Proc.PeakWorkingSet64,Proc.PrivilegedProcessorTime,Proc.TotalProcessorTime));
		}
		#endregion
	}
}
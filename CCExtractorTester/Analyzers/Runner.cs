using System;
using System.Diagnostics;
using System.Threading;

namespace CCExtractorTester
{
	public class Runner
	{
		private String CCExtractorLocation { get; set ; }
		private ILogger Logger { get; set; }
		private IPerformanceLogger PerformanceLogger { get; set; }

		public Runner (String ccextractorLocation,ILogger logger,IPerformanceLogger performanceLogger)
		{
			CCExtractorLocation = ccextractorLocation;
			Logger = logger;
			PerformanceLogger = performanceLogger;
		}

		public RunData Run(string command,DataReceivedEventHandler processError,DataReceivedEventHandler processOutput){
			ProcessStartInfo psi = new ProcessStartInfo(CCExtractorLocation);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;
			psi.Arguments = command;

			Logger.Debug ("Passed arguments: "+psi.Arguments);

			Process p = new Process ();
			p.StartInfo = psi;
			p.ErrorDataReceived += processError;
			p.OutputDataReceived += processOutput;
			p.Start ();

			PerformanceLogger.SetUp (Logger, p);

			p.BeginOutputReadLine ();
			p.BeginErrorReadLine ();
			while (!p.HasExited) {
				PerformanceLogger.DebugValue ();
				Thread.Sleep (100);
			}
			Logger.Debug ("Process Exited. Exit code: " + p.ExitCode);
			PerformanceLogger.DebugStats ();

			return new RunData(){
				ExitCode = p.ExitCode,
				Runtime = p.ExitTime-p.StartTime
			};
		}
	}
}
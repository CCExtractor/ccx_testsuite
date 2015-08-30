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

		public RunData Run(string command,DataReceivedEventHandler processError,DataReceivedEventHandler processOutput,int timeOut=180){
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

			bool canRun = true;

			System.Timers.Timer t = new System.Timers.Timer (timeOut * 1000);
			t.AutoReset = false;
			t.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e) {
				canRun = false;
			};
			t.Start ();
			while (!p.HasExited && canRun) {
				PerformanceLogger.DebugValue ();
				Thread.Sleep (100);
			}
			if (!p.HasExited) {
				Logger.Warn ("Aborting CCExtractor, maximum time elapsed.");
				p.CancelErrorRead ();
				p.CancelOutputRead ();
				p.Kill ();
				return new RunData () {
					ExitCode = -1,
					Runtime = new TimeSpan (0, 0, timeOut)
				};
			} else {
				Logger.Debug ("Process Exited. Exit code: " + p.ExitCode);
				PerformanceLogger.DebugStats ();

				return new RunData () {
					ExitCode = p.ExitCode,
					Runtime = p.ExitTime - p.StartTime
				};
			}
		}
	}
}
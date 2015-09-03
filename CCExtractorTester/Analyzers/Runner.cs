using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CCExtractorTester
{
    public class Runner
    {
        private String CCExtractorLocation { get; set; }
        private ILogger Logger { get; set; }
        private IPerformanceLogger PerformanceLogger { get; set; }

        public Runner(String ccextractorLocation, ILogger logger, IPerformanceLogger performanceLogger)
        {
            CCExtractorLocation = ccextractorLocation;
            Logger = logger;
            PerformanceLogger = performanceLogger;
        }

        public RunData Run(string command, DataReceivedEventHandler processError, DataReceivedEventHandler processOutput, int timeOut, bool useStdin = false, string inputFile = null)
        {
            ProcessStartInfo psi = new ProcessStartInfo(CCExtractorLocation);
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = useStdin;
            psi.CreateNoWindow = true;
            psi.Arguments = command;

            Logger.Debug("Passed arguments: " + psi.Arguments);

            Process p = new Process();
            p.StartInfo = psi;
            p.ErrorDataReceived += processError;
            p.OutputDataReceived += processOutput;

            p.Start();

            PerformanceLogger.SetUp(Logger, p);

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            bool canRun = true;

            System.Timers.Timer t = new System.Timers.Timer(timeOut * 1000);
            t.AutoReset = false;
            t.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                canRun = false;
            };
            t.Start();
            // If we use stdin, we need to pipe as much as possible during the execution run, otherwise we just sleep in a loop.
            if (useStdin && !String.IsNullOrEmpty(inputFile))
            {
                using (StreamWriter sw = p.StandardInput)
                {
                    using (FileStream sr = File.OpenRead(inputFile))
                    {
                        byte[] buffer = new byte[32768];
                        int read;
                        while (canRun && (read = sr.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            PerformanceLogger.DebugValue();
                            // Need to write to the basestream directly, as we have a byte buffer.
                            sw.BaseStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
            else
            {
                while (!p.HasExited && canRun)
                {
                    PerformanceLogger.DebugValue();
                    Thread.Sleep(100);
                }
            }
            // If the process hasn't exited now, it means the timer elapsed, so we need to abort CCExtractor
            if (!p.HasExited)
            {
                Logger.Warn("Aborting CCExtractor, maximum time elapsed.");
                p.CancelErrorRead();
                p.CancelOutputRead();
                p.Kill();
                return new RunData()
                {
                    ExitCode = -1,
                    Runtime = new TimeSpan(0, 0, timeOut)
                };
            }
            else
            {
                Logger.Debug("Process Exited. Exit code: " + p.ExitCode);
                PerformanceLogger.DebugStats();

                return new RunData()
                {
                    ExitCode = p.ExitCode,
                    Runtime = p.ExitTime - p.StartTime
                };
            }
        }
    }
}
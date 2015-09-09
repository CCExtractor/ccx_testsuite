using CCExtractorTester.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CCExtractorTester.Analyzers
{
    /// <summary>
    /// This class calls CCExtractor with an input file (or other method), a set of parameters and output files.
    /// </summary>
    public class Processor
    {
        public ILogger Logger { get; private set; }

        public IPerformanceLogger PerformanceLogger { get; private set; }

        public ConfigManager Config { get; private set; }

        public TestEntry Test { get; private set; }

        public Processor(ILogger logger, IPerformanceLogger performanceLogger, ConfigManager cm)
        {
            Logger = logger;
            PerformanceLogger = performanceLogger;
            Config = cm;
        }

        public RunData CallCCExtractor(TestEntry test)
        {
            Test = test;

            string commandToPass = String.Format("{0} --no_progress_bar",test.Command);
            string inputFile = Path.Combine(Config.SampleFolder, test.InputFile);

            switch (test.OutputFormat)
            {
                case OutputType.File:
                    // Append file as -o
                    // TODO: finish
                    break;
                case OutputType.Null:
                    // No output file necessary
                    break;
                case OutputType.Tcp:
                    // We'll need to set up another instance to receive the captions
                    // TODO: finish
                    break;
                case OutputType.Cea708:
                    // use -o for base filename determination
                    // TODO: finish
                    break;
                case OutputType.Multiprogram:
                    // use -o for base filename determination
                    // TODO: finish
                    break;
                case OutputType.Stdout:
                    // No output file necessary
                    break;
                default:
                    break;
            }

            ProcessStartInfo psi = new ProcessStartInfo(Config.CCExctractorLocation);
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = true;

            switch (test.InputFormat)
            {
                case InputType.File:
                    // Append input file regularly
                    commandToPass += " " + test.InputFile; 
                    break;
                case InputType.Stdin:
                    // No input file to append, but we'll have to add a handler
                    psi.RedirectStandardInput = true;
                    break;
                case InputType.Udp:
                    // Set up something to pass udp to ccextractor
                    // TODO: finish
                    break;
                default:
                    break;
            }

            psi.Arguments = commandToPass;

            // TODO: finish & revamp
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

            System.Timers.Timer t = new System.Timers.Timer(Config.TimeOut * 1000);
            t.AutoReset = false;
            t.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                canRun = false;
            };
            t.Start();
            // If we use stdin, we need to pipe as much as possible during the execution run, otherwise we just sleep in a loop.
            if (test.InputFormat == InputType.Stdin && !String.IsNullOrEmpty(test.InputFile))
            {
                using (StreamWriter sw = p.StandardInput)
                {
                    using (FileStream sr = File.OpenRead(test.InputFile))
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
            RunData rd = new RunData()
            {
                ExitCode = -1,
                Runtime = new TimeSpan(0, 0, Config.TimeOut)
            };
            // If the process hasn't exited now, it means the timer elapsed, so we need to abort CCExtractor
            if (!p.HasExited)
            {
                Logger.Warn("Aborting CCExtractor, maximum time elapsed.");
                p.CancelErrorRead();
                p.CancelOutputRead();
                p.Kill();
            }
            else
            {
                Logger.Debug("Process Exited. Exit code: " + p.ExitCode);
                PerformanceLogger.DebugStats();
                rd.ExitCode = p.ExitCode;
                rd.Runtime = p.ExitTime - p.StartTime;
                
            }
            return rd;
        }

        /// <summary>
        /// Processes the output received from CCExtractor.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void processOutput(object sender, DataReceivedEventArgs e)
        {
            if (Test.OutputFormat == OutputType.Stdout)
            {
                // Capture output and save in a file.
                using (StreamWriter sw = new StreamWriter(File.Open(Path.Combine(Config.TemporaryFolder, "stdout.file"), FileMode.OpenOrCreate)))
                {
                    sw.WriteLine(e.Data);
                }
            }
            else
            {
                Logger.Debug(e.Data);
            }
        }

        /// <summary>
        /// Processes the errors received from CCExtractor.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void processError(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                Logger.Error(e.Data);
            }
        }
    }
}
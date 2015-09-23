using CCExtractorTester.Enums;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// The logger instance that is used.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// The performance logger instance that is used.
        /// </summary>
        public IPerformanceLogger PerformanceLogger { get; private set; }

        /// <summary>
        /// The config that is used.
        /// </summary>
        public ConfigManager Config { get; private set; }

        /// <summary>
        /// The current test entry that is being processed.
        /// </summary>
        public TestEntry Test { get; private set; }

        /// <summary>
        /// The handler that will handle stdout output from the CCExtractor process.
        /// </summary>
        public DataReceivedEventHandler OutputHandler { get; set; }

        /// <summary>
        /// Creates a new instance of the processor class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="performanceLogger">The performance logger to use.</param>
        /// <param name="cm">The config that will be used.</param>
        public Processor(ILogger logger, IPerformanceLogger performanceLogger, ConfigManager cm)
        {
            Logger = logger;
            PerformanceLogger = performanceLogger;
            Config = cm;
            OutputHandler = null;
        }

        /// <summary>
        /// Executes CCExtractor, using the given test entry.
        /// </summary>
        /// <param name="test">The test entry data.</param>
        /// <param name="storeName">The name of the report.</param>
        /// <returns>An instance of RunData containing the test results.</returns>
        public RunData CallCCExtractor(TestEntry test, string storeName)
        {
            Test = test;

            string commandToPass = String.Format("{0} --no_progress_bar",test.Command);
            string inputFile = Path.Combine(Config.SampleFolder, test.InputFile);
            string firstOutputFile = Path.Combine(Config.TemporaryFolder, test.CompareFiles[0].CorrectFile);

            FileInfo firstOutputFileFI = new FileInfo(firstOutputFile);

            if (!firstOutputFileFI.Directory.Exists)
            {
                firstOutputFileFI.Directory.Create();
            }

            switch (test.OutputFormat)
            {
                case OutputType.File:
                    // Append the first file as -o
                    commandToPass += String.Format(@" -o ""{0}""", firstOutputFile);
                    break;
                case OutputType.Null:
                    // No output file necessary
                    break;
                case OutputType.Tcp:
                    // We'll need to set up another instance to receive the captions, but we'll do this later
                    commandToPass = String.Format("-sendto 127.0.0.1:{0} --no_progress_bar", Config.TCPPort);
                    break;
                case OutputType.Cea708:
                    // use -o for base determination & 608 contents
                    commandToPass += String.Format(@" -o ""{0}""", firstOutputFile);
                    break;
                case OutputType.Multiprogram:
                    // use -o for base filename determination
                    commandToPass += String.Format(@" -o ""{0}""", firstOutputFile);
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
                    commandToPass += String.Format(@" ""{0}""", inputFile);
                    break;
                case InputType.Stdin:
                    // No input file to append, but we'll have to add a handler
                    psi.RedirectStandardInput = true;
                    break;
                case InputType.Udp:
                    // Set up ffmpeg to pass udp to ccextractor later, add -udp to CCExtractor
                    commandToPass += String.Format(" -udp {0}", Config.UDPPort);
                    break;
                default:
                    break;
            }

            psi.Arguments = commandToPass;

            Logger.Debug("Passed arguments: " + psi.Arguments);

            Process p = new Process();
            p.StartInfo = psi;
            p.ErrorDataReceived += processError;
            if(OutputHandler != null)
            {
                p.OutputDataReceived += OutputHandler;
            }
            else
            {
                p.OutputDataReceived += processOutput;
            }
            

            p.Start();

            // Determine if we need to setup a sender process
            Process input = new Process();
            bool input_started = false;
            if (test.InputFormat == InputType.Udp)
            {
                // Set up ffmpeg to stream a file to CCExtractor
                input.StartInfo = new ProcessStartInfo(Config.FFMpegLocation)
                {
                    Arguments = String.Format(@"-re -i ""{0}"" -loglevel quiet -codec copy -f mpegts udp://127.0.0.1:{1}", inputFile, Config.UDPPort),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                input_started = input.Start();
            }
            // Determine if we need to setup a receiver process
            Process output = new Process();
            bool output_started = false;
            if (test.OutputFormat == OutputType.Tcp)
            {
                // Set up another CCExtractor instance to receive raw caption data
                output.StartInfo = new ProcessStartInfo(Config.CCExctractorLocation)
                {
                    Arguments = String.Format(@"{0} -tcp {1} -o ""{2}""", test.Command, Config.TCPPort, firstOutputFile),
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                output_started = output.Start();
            }
            // Set up performance logger
            PerformanceLogger.SetUp(Logger, p);
            // Start reading output of the main CCExtractor instance
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            // Create timer and start loop
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
                bool extra = true;
                while (!p.HasExited && canRun && extra)
                {
                    if(test.InputFormat == InputType.Udp)
                    {
                        extra = !input.HasExited;
                    }
                    if(test.OutputFormat == OutputType.Tcp)
                    {
                        extra = !output.HasExited;
                    }
                    PerformanceLogger.DebugValue();
                    Thread.Sleep(100);
                }
            }
            // Process results
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
            // Preventively kill off any possible other processes
            if (input_started && !input.HasExited)
            {
                input.Kill();
            }
            if (output_started && !output.HasExited)
            {
                output.Kill();
            }

            // Create store folder if necessary
            string storeDirectory = Path.Combine(Config.TemporaryFolder, storeName);
            if (!Directory.Exists(storeDirectory))
            {
                Directory.CreateDirectory(storeDirectory);
            }
            // Add generated output file(s), if any, to the result
            rd.ResultFiles = new Dictionary<string, string>();
            // Check for each expected output file if there's a generated one
            foreach (CompareFile compareFile in test.CompareFiles)
            {
                if (!compareFile.IgnoreOutput)
                {
                    FileInfo fileLocation = new FileInfo(Path.Combine(Config.TemporaryFolder, compareFile.ExpectedFile));
                    string moveLocation = Path.Combine(storeDirectory, (new FileInfo(Path.Combine(Config.TemporaryFolder,compareFile.CorrectFile)).Name));
                    if (fileLocation.Exists)
                    {
                        rd.ResultFiles.Add(compareFile.CorrectFile, moveLocation);
                        // Move file
                        if (File.Exists(moveLocation))
                        {
                            File.Delete(moveLocation);
                        }
                        fileLocation.MoveTo(moveLocation);
                    }
                    else
                    {
                        rd.ResultFiles.Add(compareFile.CorrectFile, "");
                    }
                }
            }
            // Return the results
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
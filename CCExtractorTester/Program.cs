using System;
using System.IO;
using System.Xml;
using CommandLine;
using CommandLine.Text;
using System.Reflection;
using CCExtractorTester.Enums;
using CCExtractorTester.Analyzers;

namespace CCExtractorTester
{
    /// <summary>
    /// The Options that are available for use in the command line interface.
    /// </summary>
    class Options
    {
        [Option('m', "method", DefaultValue = RunType.Report , HelpText = "How should the testsuite behave for reporting?")]
        public RunType RunMethod { get; set; }
        [Option('u', "url", HelpText = "If the method is Server, this should point to the url where the suite should send requests to")]
        public string ReportURL { get; set; }
        [Option('e', "entries", HelpText = "A XML file containing the test entries")]
        public string EntryFile { get; set; }
        [Option('c', "config", HelpText = "A XML file that contains the configuration")]
        public string ConfigFile { get; set; }
        [Option('d',"debug", DefaultValue = false, Required = false, HelpText = "Enable debugging (extra output)")]
        public bool Debug { get; set; }
        [Option('t', "tempfolder", HelpText = "Uses the provided location as a temp folder to store the results in")]
        public string TempFolder { get; set; }
        [Option('b',"breakonchanges", DefaultValue = false, Required = false, HelpText = "Break if a change in output (between generated output and correct output) is detected")]
        public bool BreakOnChanges { get; set; }
        [Option("tcp", DefaultValue = 5099, HelpText = "Sets the TCP port that will be used in case of entries that need TCP")]
        public int TCPPort { get; set; }
        [Option("udp", DefaultValue = 5099, HelpText = "Sets the UDP port that will be used in case of entries that need UDP")]
        public int UDPPort { get; set; }
        // Options that will override the config settings
        [Option("executable", HelpText = "Overrrides the CCExtractor executable path")]
        public string CCExtractorExecutable { get; set; }
        [Option("ffmpeg", HelpText = "Overrides the set FFMpeg executable path")]
        public string FFMpegExecutable { get; set; }
        [Option("reportfolder", HelpText = "Overrides the folder location where reports will be stored")]
        public string ReportFolder { get; set; }
        [Option("samplefolder", HelpText = "Overrides the folder location that contains the samples")]
        public string SampleFolder { get; set; }
        [Option("resultfolder", HelpText = "Overrides the folder location that contains the correct results")]
        public string ResultFolder { get; set; }
        [Option("comparer", HelpText = "Overrides the type of comparer that will be used")]
        public string Comparer { get; set; }
        [Option("timeout", DefaultValue = 180, HelpText = "Overrides the timeout value (default 180 seconds). This indicates how long a single test entry may take to complete. Minimum duration is 60 seconds.")]
        public int TimeOut { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption(HelpText = "Shows this screen")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    /// <summary>
    /// Main class of the program.
    /// </summary>
    class MainClass
    {
        /// <summary>
        /// The logger used to log the things that happen while this program runs.
        /// Defaults to the Console + File logger.
        /// </summary>
        public static ILogger Logger = null;

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            String location = a.Location;
            location = location.Remove(location.LastIndexOf(Path.DirectorySeparatorChar));
            Logger = new ConsoleFileLogger(Path.Combine(location, "logs"));
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                Logger.Info("Starting test suite for CCExtractor - If you encounter any issues using this program, don't hesitate to ask questions or report problems on GitHub. Please don't forget to attach logs (enable extensive logging using the -debug flag).");
                Logger.Info("Test suite version: " + a.GetName().Version.ToString());
                if (options.Debug)
                {
                    Logger.ActivateDebug();
                    Logger.Debug("Debug activated");
                }
                Logger.Info("");
                Logger.Info("If you want to see the available flags, run this program with --help. Press ctrl-c to abort if necessary.");
                Logger.Info("");
                // Loading configuration
                ConfigManager config = null;
                if (!String.IsNullOrEmpty(options.ConfigFile) && options.ConfigFile.EndsWith(".xml") && File.Exists(options.ConfigFile))
                {
                    Logger.Info("Loading provided configuration (" + options.ConfigFile + ")");
                    XmlDocument doc = new XmlDocument();
                    doc.Load(options.ConfigFile);
                    config = ConfigManager.CreateFromXML(Logger, doc);
                }
                else
                {
                    Logger.Warn("Provided no config or an invalid one; reverting to default config (" + a.GetName().Name + ".exe.config)");
                    config = ConfigManager.CreateFromAppSettings(Logger);
                }
                if(config  == null || !config.IsValidConfig())
                {
                    Logger.Error("Fatal error - config not valid. Please check. Exiting application");
                    return;
                }
                String temporaryFolder = Path.Combine(location, "tmpFiles");
                if (!String.IsNullOrEmpty(options.TempFolder) && IsValidDirectory(options.TempFolder))
                {
                    temporaryFolder = options.TempFolder;
                }
                else
                {
                    if (!Directory.Exists(temporaryFolder))
                    {
                        Logger.Warn(temporaryFolder + " does not exist; trying to create it");
                        DirectoryInfo di = Directory.CreateDirectory(temporaryFolder);
                        if (!di.Exists)
                        {
                            Logger.Error("Failed to create the directory! Exiting");
                            return;
                        }
                    }
                }
                config.TemporaryFolder = temporaryFolder;
                Logger.Info("Generated result files will be stored in " + temporaryFolder + "(when running multiple tests after another, files might be overwritten)");
                // See what overrides are specified
                if (!String.IsNullOrEmpty(options.CCExtractorExecutable))
                {
                    if (File.Exists(options.CCExtractorExecutable))
                    {
                        config.CCExctractorLocation = options.CCExtractorExecutable;
                        Logger.Info("Overriding CCExtractorLocation with given version (located at: " + options.CCExtractorExecutable + ")");
                    }
                    else
                    {
                        Logger.Error("Given CCExtractor executable path does not exist. Exiting application");
                        return;
                    }
                }
                if (options.TimeOut > 60 && options.TimeOut != 180)
                {
                    config.TimeOut = options.TimeOut;
                    Logger.Info("Overriding timeout with: " + options.TimeOut);
                }
                if (!String.IsNullOrEmpty(options.Comparer))
                {
                    try
                    {
                        config.Comparer = CompareTypeParser.parseString(options.Comparer);
                        Logger.Info("Overriding Comparer with: " + options.Comparer);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Logger.Warn("Provided value for comparer invalid; ignoring it.");
                    }
                    
                }
                if (!String.IsNullOrEmpty(options.ReportFolder))
                {
                    config.ReportFolder = options.ReportFolder;
                    Logger.Info("Overriding ReportFolder with: " + options.ReportFolder);
                }
                if (!String.IsNullOrEmpty(options.ResultFolder))
                {
                    config.ResultFolder = options.ResultFolder;
                    Logger.Info("Overriding ResultFolder with: " + options.ResultFolder);
                }
                if (!String.IsNullOrEmpty(options.SampleFolder))
                {
                    config.SampleFolder = options.SampleFolder;
                    Logger.Info("Overriding SampleFolder with: " + options.SampleFolder);
                }
                config.BreakOnChanges = options.BreakOnChanges;
                if (config.BreakOnChanges)
                {
                    Logger.Info("If there's a sample that doesn't match, we will exit instead of running them all.");
                }
                if (!String.IsNullOrEmpty(options.FFMpegExecutable))
                {
                    if (File.Exists(options.FFMpegExecutable))
                    {
                        config.FFMpegLocation = options.FFMpegExecutable;
                        Logger.Info("Overriding FFMpeg executable with given version (located at: " + options.FFMpegExecutable + ")");
                    }
                    else
                    {
                        Logger.Error("Given CCExtractor executable path does not exist. Exiting application");
                        return;
                    }
                }
                config.TestType = options.RunMethod;
                // Continue with parameter parsing
                if (config.TestType == RunType.Matrix)
                {
                    Logger.Info("Running in report mode, generating matrix");
                    if (IsValidDirectory(options.EntryFile))
                    {
                        StartMatrixGenerator(Logger, config, options.EntryFile);
                    }
                    else
                    {
                        Logger.Error("Invalid directory provided for matrix generation!");
                    }
                }
                else if (IsValidPotentialSampleFile(options.EntryFile))
                {
                    Logger.Info("Running provided file");
                    StartTester(Logger, config, options.EntryFile, location);
                }
                else
                {
                    Logger.Error("No file (or invalid file) provided!");
                }
            }
        }

        /// <summary>
        /// Determines if the provided sampleFile is a valid potential sample file. Checks if it's not empty, has an xml extension and if the file exists.
        /// </summary>
        /// <returns><c>true</c> if the file exists, has an xml extension and the string is not null or empty; otherwise, <c>false</c>.</returns>
        /// <param name="sampleFile">Sample file.</param>
        static bool IsValidPotentialSampleFile(string sampleFile)
        {
            return (!String.IsNullOrEmpty(sampleFile) && sampleFile.EndsWith(".xml") && File.Exists(sampleFile));
        }

        /// <summary>
        /// Determines if the given directory is a valid and existing directory.
        /// </summary>
        /// <returns><c>true</c> if it is a valid and existing directory; otherwise, <c>false</c>.</returns>
        /// <param name="directory">The directory string to check.</param>
        static bool IsValidDirectory(string directory)
        {
            return (!String.IsNullOrEmpty(directory) && Directory.Exists(directory));
        }

        /// <summary>
        /// Starts the tester.
        /// </summary>
        /// <param name="logger">The logger that will be used by the tester.</param>
        /// <param name="config">The configuration that will be used by the tester.</param>
        /// <param name="sampleFile">The sample file the tester will be running.</param>
        /// <param name="location">The directory this executable resides in.</param>
        static void StartTester(ILogger logger, ConfigManager config, string sampleFile, string location)
        {
            Tester t = new Tester(logger, config, sampleFile);
            t.SetProgressReporter(new ConsoleReporter(logger));
            try
            {
                t.RunTests(location);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        /// <summary>
        /// Starts the matrix generation.
        /// </summary>
        /// <param name="logger">The logger that will be used by the generator.</param>
        /// <param name="config">The configuration that will be used by the generator.</param>
        /// <param name="searchFolder">The folder containing the media samples.</param>
        static void StartMatrixGenerator(ILogger logger, ConfigManager config, string searchFolder)
        {
            MatrixGenerator generator = new MatrixGenerator(logger, config, searchFolder);
            generator.ProgressReporter = new ConsoleReporter(logger);
            try
            {
                generator.GenerateMatrix();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        // An internal class for logging progress to the console.
        class ConsoleReporter : IProgressReportable
        {
            private ILogger Logger { get; set; }

            public ConsoleReporter(ILogger logger)
            {
                Logger = logger;
            }

            #region IProgressReportable implementation

            public void showProgressMessage(string message)
            {
                Logger.Info(message);
            }
            #endregion
        }
    }
}
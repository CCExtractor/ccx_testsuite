using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace CCExtractorTester
{
    using Enums;

    /// <summary>
    /// The class that does the heavy lifting in this application.
    /// </summary>
    public class Tester
    {
        /// <summary>
        /// The name of the loaded file.
        /// </summary>
        public String LoadedFileName { get; private set; }

        /// <summary>
        /// A list of the TestEntry instances that will be processed.
        /// </summary>
        public List<TestEntry> Entries { get; private set; }

        /// <summary>
        /// Contains the multi test list.
        /// </summary>
        public List<string> MultiTest { get; private set; }

        /// <summary>
        /// Gets or sets the generation of the tests that will be ran.
        /// </summary>
        public string TestGeneration { get; private set; }

        /// <summary>
        /// Gets or sets the progress reporter that will be used.
        /// </summary>
        private IProgressReportable ProgressReporter { get; set; }

        /// <summary>
        /// Gets or sets the comparer that will be used by the tester.
        /// </summary>
        private IFileComparable Comparer { get; set; }

        /// <summary>
        /// Gets or sets the configuration instance that will be used.
        /// </summary>
        private ConfigManager Config { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the performance logger.
        /// </summary>
        private IPerformanceLogger PerformanceLogger { get; set; }

        /// <summary>
        /// Indicates a first generation file (tests).
        /// </summary>
        const string XML_FIRST_GENERATION = "first_gen";
        /// <summary>
        /// Indicates a second generation file (multitests).
        /// </summary>
        const string XML_SECOND_GENERATION = "second_gen";
        /// <summary>
        /// Indicates a third generation file (testsuite).
        /// </summary>
        const string XML_THIRD_GENERATION = "third_gen";

        /// <summary>
        /// Initializes a new instance of the Tester class.
        /// </summary>
        /// <param name="logger">The logger that will be used.</param>
        /// <param name="config">The configuration that will be used.</param>
        public Tester(ILogger logger, ConfigManager config)
        {
            Entries = new List<TestEntry>();
            MultiTest = new List<String>();
            ProgressReporter = NullProgressReporter.Instance;
            Config = config;
            Logger = logger;
            LoadPerformanceLogger();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCExtractorTester.Tester"/> class.
        /// </summary>
        /// <param name="logger">The logger that will be used.</param>
        /// <param name="config">The configuration that will be used.</param>
        /// <param name="xmlFile">The XML file containing all test entries.</param>
        public Tester(ILogger logger, ConfigManager config, string xmlFile) : this(logger, config)
        {
            if (!String.IsNullOrEmpty(xmlFile))
            {
                loadAndParseXML(xmlFile);
            }
        }

        /// <summary>
        /// Sets the comparer based on the "Comparer" config setting.
        /// </summary>
        void LoadComparer()
        {
            switch (Config.Comparer)
            {
                case CompareType.Diff:
                    Comparer = new DiffLinuxComparer();
                    break;
                case CompareType.Diffplex:
                    Comparer = new DiffToolComparer(false);
                    break;
                case CompareType.Diffplexreduced:
                    Comparer = new DiffToolComparer(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("comparer", "The comparer has an illegal value!");
            }
        }

        /// <summary>
        /// Loads the performance logger, based on the used platform.
        /// </summary>
        void LoadPerformanceLogger()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    PerformanceLogger = new WindowsPerformanceCounters();
                    break;
                default:
                    PerformanceLogger = NullPerformanceLogger.Instance;
                    break;
            }

        }

        /// <summary>
        /// Loads the given XML file and tries to parse it into XML.
        /// </summary>
        /// <param name="xmlFileName">The location of the XML file to load and parse.</param>
        void loadAndParseXML(string xmlFileName)
        {
            if (File.Exists(xmlFileName))
            {
                TestGeneration = ValidateXML(xmlFileName);
                XmlDocument doc = new XmlDocument();
                using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
                {
                    doc.Load(fs);
                    FileInfo fi = new FileInfo(xmlFileName);
                    switch (TestGeneration)
                    {
                        case XML_FIRST_GENERATION:
                            // Dealing with the first typee, test(s) only
                            XmlNodeList testNodes = doc.SelectNodes("//test");
                            LoadedFileName = fi.Name.Replace(".xml", "");
                            foreach (XmlNode node in testNodes)
                            {
                                XmlNode sampleFile = node.SelectSingleNode("sample");
                                XmlNode command = node.SelectSingleNode("cmd");
                                XmlNode resultFile = node.SelectSingleNode("result");
                                Entries.Add(
                                    new TestEntry(
                                        ConvertFolderDelimiters(sampleFile.InnerText), 
                                        command.InnerText, 
                                        ConvertFolderDelimiters(resultFile.InnerText)
                                    )
                                );
                            }
                            break;
                        case XML_SECOND_GENERATION:
                            // Dealing with multi file
                            foreach (XmlNode node in doc.SelectNodes("//testfile"))
                            {
                                String testFileLocation = ConvertFolderDelimiters(node.SelectSingleNode("location").InnerText);
                                testFileLocation = Path.Combine(fi.DirectoryName, testFileLocation);
                                MultiTest.Add(testFileLocation);
                            }
                            break;
                        case XML_THIRD_GENERATION:
                            // Dealing with the newest version of tests
                            foreach (XmlNode node in doc.SelectNodes("//entry"))
                            {
                                // Get nodes for entry
                                XmlNode command = node.SelectSingleNode("command");
                                XmlNode input = node.SelectSingleNode("input");
                                XmlNode output = node.SelectSingleNode("output");
                                XmlNodeList compareTo = node.SelectSingleNode("compare").SelectNodes("out");
                                // Convert to appropriate values
                                string ccx_command = command.InnerText;
                                string inputFile = input.InnerText;
                                InputType inputType = InputTypeParser.parseString(input.Attributes["type"].Value);
                                OutputType outputType = OutputTypeParser.parseString(output.InnerText);
                                List<string> compareFiles = new List<string>();
                                foreach(XmlNode n in compareTo)
                                {
                                    compareFiles.Add(n.InnerText);
                                }
                                // Add entry
                                Entries.Add(new TestEntry(inputFile, inputType, ccx_command, outputType, compareFiles));
                            }
                            break;
                        default:
                            break;
                    }
                }
                return;
            }
            throw new InvalidDataException("File does not exist");
        }

        /// <summary>
        /// Validates the given XML at the location against our scheme.
        /// </summary>
        /// <param name="xmlFileName">The location of the XML file to validate.</param>
        /// <returns>A string that indicates the generation of the XML file.</returns>
        string ValidateXML(string xmlFileName)
        {
            Dictionary<string, string> schemes = new Dictionary<string, string>() {
                { XML_THIRD_GENERATION, Resources.testsuite },
                { XML_SECOND_GENERATION, Resources.multitest },
                { XML_FIRST_GENERATION, Resources.tests }
            };

            foreach (KeyValuePair<string, string> kvp in schemes)
            {
                try
                {
                    ValidateAgainstSchema(xmlFileName, kvp.Value);
                    return kvp.Key;
                }
                catch (XmlSchemaValidationException)
                {
                    continue;
                }
            }

            throw new InvalidDataException("Given XML is not a valid file for the possible formats.");
        }

        /// <summary>
        /// Validates the XML against a given schema.
        /// </summary>
        /// <param name="xmlFileName">Xml file name.</param>
        /// <param name="xmlSchema">Xml schema.</param>
        private void ValidateAgainstSchema(string xmlFileName, string xmlSchema)
        {
            using (StringReader sr = new StringReader(xmlSchema))
            {
                XmlReader r = XmlReader.Create(sr);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, r);
                settings.ValidationType = ValidationType.Schema;
                using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
                {
                    var reader = XmlReader.Create(fs, settings);
                    while (reader.Read())
                    {
                        // Nothing in here, just need to read out the entire file in a loop.
                    }
                }
            }
        }

        /// <summary>
        /// Converts the folder delimiters between platforms to ensure no issues when running test xmls created on another platform.
        /// </summary>
        /// <returns>The converted path.</returns>
        /// <param name="path">The path to check and if necessary, convert.</param>
        string ConvertFolderDelimiters(string path)
        {
            char env = '\\';
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    env = '/';
                    break;
                default:
                    break;
            }
            return path.Replace(env, Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="location">The folder this executable resides in.</param>
        public void RunTests(string location)
        {
            String cce = Config.CCExctractorLocation;
            if (!File.Exists(cce))
            {
                throw new InvalidOperationException("CCExtractor location (" + cce + ") is not a valid file/executable");
            }
            String sourceFolder = Config.SampleFolder;
            if (!Directory.Exists(sourceFolder))
            {
                throw new InvalidOperationException("Sample folder does not exist!");
            }

            bool useThreading = Config.Threading;
            if (useThreading)
            {
                Logger.Info("Using threading");
            }

            if (MultiTest.Count > 0)
            {
                // Override ReportFolder and create subdirectory for it if necessary
                String subFolder = Path.Combine(Config.ReportFolder, "Testsuite_Report_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }
                Logger.Info("Multitest, overriding report folder to: " + subFolder);
                Config.ReportFolder = subFolder;
                // Run through test files
                StringBuilder sb = new StringBuilder();
                foreach (string s in MultiTest)
                {
                    Entries.Clear();
                    loadAndParseXML(s);
                    int nrTests = Entries.Count;
                    Tuple<int, string> singleTest = RunSingleFileTests(useThreading, cce, location, sourceFolder);
                    sb.AppendFormat(
                        @"<tr><td><a href=""{0}"">{1}</a></td><td class=""{2}"">{3}/{4}</td></tr>",
                        singleTest.Item2, LoadedFileName,
                        (singleTest.Item1 == nrTests) ? "green" : "red",
                        singleTest.Item1, nrTests
                    );
                    SaveMultiIndexFile(sb.ToString(), subFolder);
                    if (singleTest.Item1 != nrTests && Config.BreakOnChanges)
                    {
                        Logger.Info("Aborting next files because of error in current test file");
                        break;
                    }
                }
            }
            else
            {
                RunSingleFileTests(useThreading, cce, location, sourceFolder);
            }
        }

        /// <summary>
        /// Saves the index file for the multi-test, given the current entries that have been completed so far.
        /// </summary>
        /// <param name="currentEntries">The HTML for the current entries that have been completed already.</param>
        /// <param name="subFolder">The location of the folder where the index should be written to.</param>
        private void SaveMultiIndexFile(string currentEntries, string subFolder)
        {
            StringBuilder sb = new StringBuilder(@"
				<html>
					<head>
						<title>Test suite result index</title>
						<style>.green { background-color: green; } .red { background-color: red; }</style>
					</head>
					<body>
						<table>
							<tr>
								<th>Report name</th>
								<th>Tests passed</th>
							</tr>");
            sb.Append(currentEntries);
            sb.Append("</table></body></html>");
            using (StreamWriter sw = new StreamWriter(Path.Combine(subFolder, "index.html")))
            {
                sw.WriteLine(sb.ToString());
            }
        }

        /// <summary>
        /// Runs the tests for a single test file, which can contain multiple test entries.
        /// </summary>
        /// <returns>A tuple with the number of successful tests and the location of the resulting file.</returns>
        /// <param name="useThreading">If set to <c>true</c> use threading.</param>
        /// <param name="cce">The CCExctractor location.</param>
        /// <param name="location">The location of the executable.</param>
        /// <param name="sourceFolder">The source folder with the samples.</param>
        Tuple<int, string> RunSingleFileTests(bool useThreading, String cce, String location, String sourceFolder)
        {
            LoadComparer();

            int i = 1;
            SetUpTestEntryProcessing(cce, location, sourceFolder, Entries.Count, LoadedFileName);
            ManualResetEvent[] mres = new ManualResetEvent[Entries.Count];
            DateTime start = DateTime.Now;

            foreach (TestEntry te in Entries)
            {
                mres[i - 1] = new ManualResetEvent(false);
                TestEntryProcessing tep = new TestEntryProcessing(te, i);
                tep.eventX = mres[i - 1];

                if (useThreading)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(tep.Process));
                }
                else
                {
                    tep.Process(null);
                }

                i++;
            }
            if (useThreading)
            {
                WaitHandle.WaitAll(mres);
            }
            DateTime end = DateTime.Now;
            Logger.Info("Runtime: " + (end.Subtract(start)).ToString());
            String reportName = Comparer.SaveReport(
                Config.ReportFolder,
                new ResultData()
                {
                    FileName = LoadedFileName,
                    CCExtractorVersion = cce + " on " + DateTime.Now.ToShortDateString(),
                    StartTime = start
                }
            );
            int unchanged = Comparer.GetSuccessNumber();
            Comparer = null;
            return Tuple.Create(unchanged, reportName);
        }

        /// <summary>
        /// Sets up test entry processing.
        /// </summary>
        /// <param name="cce">The CCExtractor executable location.</param>
        /// <param name="location">The folder from where this program is executed.</param>
        /// <param name="sourceFolder">The folder with the samples in.</param>
        /// <param name="total">The number of test entries to process.</param>
        /// <param name="testName">The name of the test file we're running.</param>
        void SetUpTestEntryProcessing(string cce, string location, string sourceFolder, int total, string testName)
        {
            TestEntryProcessing.location = location;
            TestEntryProcessing.sourceFolder = sourceFolder;
            TestEntryProcessing.total = total;
            TestEntryProcessing.comparer = Comparer;
            TestEntryProcessing.config = Config;
            TestEntryProcessing.logger = Logger;
            TestEntryProcessing.progressReporter = ProgressReporter;
            TestEntryProcessing.testName = testName;
            TestEntryProcessing.runner = new Runner(cce, Logger, PerformanceLogger);
        }

        /// <summary>
        /// Sets the progress reporter.
        /// </summary>
        /// <param name="progressReporter">Progress reporter.</param>
        public void SetProgressReporter(IProgressReportable progressReporter)
        {
            ProgressReporter = progressReporter;
        }

        /// <summary>
        /// Internal class that processes a single entry.
        /// </summary>
        class TestEntryProcessing
        {
            public static Runner runner;
            public static string sourceFolder;
            public static string location;
            public static int total;
            public static ILogger logger;
            public static IFileComparable comparer;
            public static IProgressReportable progressReporter;
            public static ConfigManager config;
            public static string testName;

            private TestEntry te;
            private int current;

            public ManualResetEvent eventX;

            /// <summary>
            /// Initializes a new instance of the <see cref="CCExtractorTester.Tester+TestEntryProcessing"/> class.
            /// </summary>
            /// <param name="testingEntry">Testing entry.</param>
            /// <param name="curr">Curr.</param>
            public TestEntryProcessing(TestEntry testingEntry, int curr)
            {
                te = testingEntry;
                current = curr;
            }

            /// <summary>
            /// Process the current entry.
            /// </summary>
            /// <param name="state">State.</param>
            public void Process(object state)
            {
                progressReporter.showProgressMessage(String.Format("Starting with entry {0} of {1}", current, total));

                string sampleFile = Path.Combine(sourceFolder, te.InputFile);
                string producedFileName = te.CompareFiles[0].Substring(te.CompareFiles[0].LastIndexOf(Path.DirectorySeparatorChar) + 1);
                string producedFile = Path.Combine(config.TemporaryFolder, producedFileName);
                string expectedResultFile = Path.Combine(config.ResultFolder, te.CompareFiles[0]);

                string command = te.Command + String.Format(@" --no_progress_bar -o ""{0}"" ""{1}""  ", producedFile, sampleFile);

                RunData rd = runner.Run(command, processError, processOutput, config.TimeOut);

                try
                {
                    comparer.CompareAndAddToResult(
                        new CompareData()
                        {
                            ProducedFile = producedFile,
                            CorrectFile = expectedResultFile,
                            SampleFile = sampleFile,
                            Command = te.Command,
                            RunTime = rd.Runtime,
                            ExitCode = rd.ExitCode
                        });
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }

                // Move produced file to another location (so it still can be inspected later, but won't affect next runs
                string storeDirectory = Path.Combine(config.TemporaryFolder, testName);
                if (!Directory.Exists(storeDirectory))
                {
                    Directory.CreateDirectory(storeDirectory);
                }
                string storeFile = Path.Combine(storeDirectory, producedFileName);
                if (File.Exists(storeFile))
                {
                    File.Delete(storeFile);
                }
                if (File.Exists(producedFile))
                {
                    File.Move(producedFile, storeFile);
                }

                // Report back that we finished an entry
                progressReporter.showProgressMessage(String.Format("Finished entry {0} with exit code: {1}", current, rd.ExitCode));
                eventX.Set();
            }

            /// <summary>
            /// Processes the output received from CCExtractor.
            /// </summary>
            /// <param name="sender">Sender.</param>
            /// <param name="e">E.</param>
            void processOutput(object sender, DataReceivedEventArgs e)
            {
                logger.Debug(e.Data);
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
                    logger.Error(e.Data);
                }
            }
        }
    }
}
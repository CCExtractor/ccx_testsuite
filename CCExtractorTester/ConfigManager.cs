using CCExtractorTester.Enums;
using System;
using System.Configuration;
using System.Xml;

namespace CCExtractorTester
{
    /// <summary>
    /// Holds all the configuration settings for the test suite.
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// Gets or sets the folder where we'll store the report in (when using the old local method)
        /// </summary>
        public string ReportFolder { get; set; }

        /// <summary>
        /// Gets or sets the folder where the samples are stored.
        /// </summary>
        public string SampleFolder { get; set; }

        /// <summary>
        /// Gets or sets the folder that holds the correct results to compare to.
        /// </summary>
        public string ResultFolder { get; set; }

        /// <summary>
        /// Gets or sets the location of the CCExtractor executable.
        /// </summary>
        public string CCExctractorLocation { get; set; }

        /// <summary>
        /// Gets or sets which type of comparison should be used.
        /// </summary>
        public CompareType Comparer { get; set; }

        /// <summary>
        /// Gets or sets if threading should be used.
        /// </summary>
        public bool Threading { get; set; }

        /// <summary>
        /// Gets or sets if the suite should break when it encounters an error.
        /// </summary>
        public bool BreakOnChanges { get; set; }
        
        /// <summary>
        /// Gets or sets if the suite should process the results and create reports, or pass results to the server.
        /// </summary>
        public RunType TestType { get; set; }

        /// <summary>
        /// Gets or sets the url that should be used to send the progress of the test suite to.
        /// </summary>
        public string ReportUrl { get; set; }

        /// <summary>
        /// Gets or sets the port that will be used if there's a test involving TCP connections.
        /// </summary>
        public int TCPPort { get; set; }

        /// <summary>
        /// Gets or sets the port that will be used if there's a test involving UDP connections.
        /// </summary>
        public int UDPPort { get; set; }

        /// <summary>
        /// Gets or sets the location of the folder to store temporary results in.
        /// </summary>
        public string TemporaryFolder { get; set; }

        /// <summary>
        /// Gets or sets the time out that will be used to abort a running instance of CCExctractor if necessary. (in seconds). Set by default to 180.
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// Creates a new instance of the ConfigManager.
        /// </summary>
        /// <param name="reportFolder">The folder to store reports in.</param>
        /// <param name="sampleFolder">The folder to store samples in.</param>
        /// <param name="resultFolder">The folder that holds the correct results.</param>
        /// <param name="ccextractorLocation">The location of the CCExtractor executable.</param>
        /// <param name="compare">The comparer to use for reports.</param>
        /// <param name="useThreading">Use threading?</param>
        /// <param name="breakErrors">Break when the suite encounters a broken file?</param>
        private ConfigManager(string reportFolder, string sampleFolder, string resultFolder, string ccextractorLocation, CompareType compare, bool useThreading, bool breakErrors)
        {
            ReportFolder = reportFolder;
            SampleFolder = sampleFolder;
            ResultFolder = resultFolder;
            CCExctractorLocation = ccextractorLocation;
            Comparer = compare;
            Threading = useThreading;
            BreakOnChanges = breakErrors;
            TimeOut = 180;
        }

        /// <summary>
        /// Creates a ConfigManager instance from the app.config file (if it exists).
        /// </summary>
        /// <param name="logger">The logger instance, so errors can be logged.</param>
        /// <returns>An instance of teh ConfigManager.</returns>
        public static ConfigManager CreateFromAppSettings(ILogger logger)
        {
            Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string reportFolder = "", sampleFolder = "", resultFolder = "", ccextractorLocation = "";
            CompareType compare = CompareType.Diffplex;
            bool useThreading = false, breakErrors = false;
            foreach (KeyValueConfigurationElement kce in c.AppSettings.Settings)
            {
                switch (kce.Key)
                {
                    case "ReportFolder":
                        reportFolder = kce.Value;
                        break;
                    case "SampleFolder":
                        sampleFolder = kce.Value;
                        break;
                    case "CorrectResultFolder":
                        resultFolder = kce.Value;
                        break;
                    case "CCExtractorLocation":
                        ccextractorLocation = kce.Value;
                        break;
                    case "Comparer":
                        try
                        {
                            compare = (CompareType) Enum.Parse(typeof(CompareType), kce.Value);
                        }
                        catch (ArgumentException)
                        {
                            logger.Warn("Could not parse the Comparer value from the config. Will be using the default setting");
                        }
                        break;
                    case "UseThreading":
                        useThreading = bool.Parse(kce.Value);
                        break;
                    case "BreakOnErrors":
                        breakErrors = bool.Parse(kce.Value);
                        break;
                    default:
                        logger.Warn("Unknown key " + kce.Key + " encountered; ignoring.");
                        break;
                }
            }
            return new ConfigManager(reportFolder,sampleFolder,resultFolder,ccextractorLocation,compare,useThreading,breakErrors);
        }

        /// <summary>
        /// Creates a ConfigManager instance from given xml settings file.
        /// </summary>
        /// <param name="logger">The logger instance, so errors can be logged.</param>
        /// <param name="xml">The XML document to parse.</param>
        /// <returns>An instance of teh ConfigManager.</returns>
        public static ConfigManager CreateFromXML(ILogger logger, XmlDocument xml)
        {
            string reportFolder = "", sampleFolder = "", resultFolder = "", ccextractorLocation = "";
            CompareType compare = CompareType.Diffplex;
            bool useThreading = false, breakErrors = false;
            foreach (XmlNode n in xml.SelectNodes("configuration/appSettings/add"))
            {
                string key = n.Attributes["key"].Value;
                string value = n.Attributes["value"].Value;
                switch (key)
                {
                    case "ReportFolder":
                        reportFolder = value;
                        break;
                    case "SampleFolder":
                        sampleFolder = value;
                        break;
                    case "CorrectResultFolder":
                        resultFolder = value;
                        break;
                    case "CCExtractorLocation":
                        ccextractorLocation = value;
                        break;
                    case "Comparer":
                        try
                        {
                            compare = (CompareType)Enum.Parse(typeof(CompareType), value);
                        }
                        catch (ArgumentException)
                        {
                            logger.Warn("Could not parse the Comparer value from the xml. Will be using the default setting");
                        }
                        break;
                    case "UseThreading":
                        useThreading = bool.Parse(value);
                        break;
                    case "BreakOnErrors":
                        breakErrors = bool.Parse(value);
                        break;
                    default:
                        logger.Warn("Unknown key " + key + " encountered; ignoring.");
                        break;
                }
            }
            return new ConfigManager(reportFolder, sampleFolder, resultFolder, ccextractorLocation, compare, useThreading, breakErrors);
        }

        /// <summary>
        /// Checks if the config is correctly initialized.
        /// </summary>
        /// <returns>True if the configuration is correct, false otherwise.</returns>
        public bool IsValidConfig()
        {
            bool pass = true;
            /* 
            Required in any case: sample folder, ccx location, test type, threading, errorbreak

            Only the first two could be empty or null and need to be checked.
            */
            pass = pass && !String.IsNullOrEmpty(SampleFolder);
            pass = pass && !String.IsNullOrEmpty(CCExctractorLocation);
            // Depending on the test type, other parameters are necessary
            switch (TestType)
            {
                case RunType.Report:
                    // We need a report folder, result folder, Comparer set
                    pass = pass && !String.IsNullOrEmpty(ReportFolder);
                    pass = pass && !String.IsNullOrEmpty(ResultFolder);
                    break;
                case RunType.Server:
                    // We need the report url
                    pass = pass && !String.IsNullOrEmpty(ReportUrl);
                    break;
                case RunType.Matrix:
                    // We need a report folder
                    break;
                default:
                    // Invalid type
                    pass = false;
                    break;
            }

            return pass;
        }
    }
}

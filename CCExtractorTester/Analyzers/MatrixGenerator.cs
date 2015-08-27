using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace CCExtractorTester
{
    /// <summary>
    /// This class handles the generation of a functionality matrix for a given folder.
    /// </summary>
	public class MatrixGenerator
    {
        /// <summary>
        /// Gets or sets the instance to report progress to.
        /// </summary>
        public IProgressReportable ProgressReporter { private get; set; }

        /// <summary>
        /// Gets or sets the instance that will be used for logging.
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the configuration object.
        /// </summary>
        private ConfigManager Config { get; set; }

        /// <summary>
        /// Gets or sets the folder that will be searched for files to pass on to the matrix generation part.
        /// </summary>
        private string SearchFolder { get; set; }

        /// <summary>
        /// Creates a new instance of the matrix generator class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="config">The configuration settings to use.</param>
        /// <param name="searchFolder">The folder to search.</param>
        public MatrixGenerator(ILogger logger, ConfigManager config, string searchFolder)
        {
            Logger = logger;
            Config = config;
            SearchFolder = searchFolder;
        }

        /// <summary>
        /// Generates a matrix report file by running through the folder and passing all files to CCExtractor with the -report function enabled.
        /// </summary>
        public void GenerateMatrix()
        {
            if (!File.Exists(Config.CCExctractorLocation))
            {
                throw new InvalidOperationException("CCExtractor location (" + Config.CCExctractorLocation + ") is not a valid file/executable");
            }
            Runner r = new Runner(Config.CCExctractorLocation, Logger, NullPerformanceLogger.Instance);

            StringBuilder sb = new StringBuilder();

            foreach (String path in Directory.GetFiles(SearchFolder, "*.*", SearchOption.AllDirectories))
            {
                sb.AppendLine(ProcessFile(r, path));
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(Config.ReportFolder, "Matrix.html")))
            {
                sw.WriteLine(String.Format(@"
				<html>
					<head>
						<title>{0}</title>
						<style type=""text/css"">
							table, td, th {{
								border: 2px solid black;
								border-collapse: collapse;
								padding: 5px;
								text-align: center;
							}}
							th {{
								border-bottom: 5px double;
							}}
							td:first-child,th:first-child {{
								border-right: 5px double;
							}}
						</style>
					</head>
					<body>", "Matrix generated on " + DateTime.Now.ToShortDateString() + " for " + SearchFolder));
                sw.WriteLine("<table>");
                sw.WriteLine("<tr><th>File</th><th>Stream Mode</th><th>EIA-608</th><th>CEA-708</th><th>Teletext</th><th>DVB</th><th>MPEG4 Timed Text</th><th>XDS Packets</th></tr>");
                sw.WriteLine(sb.ToString());
                sw.WriteLine("</table>");
                sw.WriteLine("</body></html>");
            }
        }

        /// <summary>
        /// Processes a single file, using given runner and given file location.
        /// </summary>
        /// <param name="runner">The runner that launches and handles the execution of CCExtractor.</param>
        /// <param name="fileLocation">The location of the input file.</param>
        /// <returns>An entry for the matrix table.</returns>
        string ProcessFile(Runner runner, string fileLocation)
        {
            Logger.Info("Processing file " + fileLocation);
            ReportData rd = new ReportData(Logger);

            DataReceivedEventHandler processError = delegate (object sender, DataReceivedEventArgs e)
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Logger.Error(e.Data);
                }
            };
            DataReceivedEventHandler processOutput = delegate (object sender, DataReceivedEventArgs e)
            {
                string data = e.Data;
                Logger.Debug("Received report line: " + data);
                if (!String.IsNullOrEmpty(data) && data.IndexOf(':') > 0)
                {
                    string[] keyval = data.Split(new char[] { ':' }, 2);
                    if (keyval.Length == 2)
                    {
                        rd.setValue(keyval[0], keyval[1]);
                    }
                    else
                    {
                        Logger.Debug("There are not 2 elements in the split data, ignoring");
                    }
                }
            };
            runner.Run(String.Format("-out=report --no_progress_bar \"{0}\"", fileLocation), processError, processOutput);
            FileInfo f = new FileInfo(fileLocation);
            return rd.CreateRow(f.Name);
        }

        class ReportData
        {
            public enum Bool
            {
                Undefined, Yes, No
            }

            private ILogger Logger { get; set; }

            public string StreamMode { get; private set; }
            public Bool Has608 { get; private set; }
            public Bool Has708 { get; private set; }
            public Bool HasDVB { get; private set; }
            public Bool HasTeletext { get; private set; }
            public Bool HasMPEG4TimedText { get; private set; }
            public Bool HasXDS { get; private set; }

            public ReportData(ILogger logger)
            {
                Logger = logger;

                StreamMode = "Unknown";
            }

            public void setValue(string key, string value)
            {
                switch (key)
                {
                    case "Stream Mode":
                        StreamMode = value;
                        break;
                    case "DVB Subtitles":
                        HasDVB = GetBoolFromString(value);
                        break;
                    case "Teletext":
                        HasTeletext = GetBoolFromString(value);
                        break;
                    case "EIA-608":
                        Has608 = GetBoolFromString(value);
                        break;
                    case "CEA-708":
                        Has708 = GetBoolFromString(value);
                        break;
                    case "MPEG-4 Timed Text":
                        HasMPEG4TimedText = GetBoolFromString(value);
                        break;
                    case "XDS":
                        HasXDS = GetBoolFromString(value);
                        break;
                    default:
                        break;
                }
            }

            public string CreateRow(string path)
            {
                return String.Format(@"
					<tr>
						<td>{0}</td>
						<td>{1}</td>
						<td>{2}</td>
						<td>{3}</td>
						<td>{4}</td>
						<td>{5}</td>
						<td>{6}</td>
						<td>{7}</td>
					</tr>"
                    , path, StreamMode, Has608, Has708, HasTeletext, HasDVB, HasMPEG4TimedText, HasXDS);
            }

            public Bool GetBoolFromString(string value)
            {
                value = value.Trim();
                switch (value)
                {
                    case "Yes":
                        return Bool.Yes;
                    case "No":
                        return Bool.No;
                    default:
                        Logger.Debug(value + " cannot be converted to a Bool value");
                        return Bool.Undefined;
                }
            }
        }
    }
}


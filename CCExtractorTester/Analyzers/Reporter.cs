using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace CCExtractorTester
{
	public class Reporter
	{
		/// <summary>
		/// Gets or sets the progress reporter that will be used.
		/// </summary>
		/// <value>The progress reporter.</value>
		private IProgressReportable ProgressReporter { get; set; }
		/// <summary>
		/// Gets or sets the configuration instance that will be used.
		/// </summary>
		/// <value>The config.</value>
		private ConfigurationSettings Config { get; set; }
		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
		private ILogger Logger { get; set; }
		/// <summary>
		/// Gets or sets the matrix.
		/// </summary>
		/// <value>The matrix.</value>
		private string Matrix { get; set; }

		public Reporter (ConfigurationSettings config, ILogger logger, string matrix)
		{
			Config = config;
			Logger = logger;
			Matrix = matrix;		
		}

		public void SetProgressReporter (IProgressReportable progressReporter)
		{
			ProgressReporter = progressReporter;
		}

		public void GenerateMatrix ()
		{
			String cce = Config.GetAppSetting ("CCExtractorLocation");
			if (!File.Exists (cce)) {
				throw new InvalidOperationException ("CCExtractor location ("+cce+") is not a valid file/executable");
			}
			Runner r = new Runner (cce, Logger,NullPerformanceLogger.Instance);

			StringBuilder sb = new StringBuilder ();

			foreach(String path in Directory.GetFiles(Matrix,"*.*",SearchOption.AllDirectories)){
				sb.AppendLine(ProcessFile (r,path));
			}

			using (StreamWriter sw = new StreamWriter (Path.Combine (Config.GetAppSetting ("ReportFolder"), "Matrix.html"))) {
				sw.WriteLine (String.Format (@"
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
					<body>", "Matrix " + DateTime.Now.ToShortDateString ()));
				sw.WriteLine ("<table>");
				sw.WriteLine("<tr><th>File</th><th>Stream Mode</th><th>EIA-608</th><th>CEA-708</th><th>Teletext</th><th>DVB</th><th>MPEG4 Timed Text</th><th>XDS Packets</th></tr>");
				sw.WriteLine (sb.ToString ());
				sw.WriteLine ("</table>");
				sw.WriteLine ("</body></html>");
			}
		}

		string ProcessFile (Runner r,string path)
		{
			Logger.Info ("Processing file " + path);
			ReportData rd = new ReportData(Logger);

			DataReceivedEventHandler processError = delegate(object sender, DataReceivedEventArgs e){
				if(!String.IsNullOrEmpty(e.Data)){
					Logger.Error(e.Data);
				}
			};
			DataReceivedEventHandler processOutput = delegate(object sender, DataReceivedEventArgs e) {
				string data = e.Data;
				Logger.Debug("Received report line: "+data);
				if(!String.IsNullOrEmpty(data) && data.IndexOf(':')>0){
					string[] keyval = data.Split(new char[]{':'},2);
					if(keyval.Length == 2){
						rd.setValue(keyval[0],keyval[1]);
					} else {
						Logger.Debug("There are not 2 elements in the split data, ignoring");
					}
				}
			};
			r.Run (String.Format("-out=report --no_progress_bar \"{0}\"",path), processError, processOutput);
			FileInfo f = new FileInfo (path);
			return rd.CreateRow (f.Name);
		}

		class ReportData {
			public enum Bool {
				Undefined,Yes,No
			}

			private ILogger Logger { get; set; }

			public string StreamMode { get; private set; }
			public Bool Has608 { get; private set; }
			public Bool Has708 { get; private set; }
			public Bool HasDVB { get; private set; }
			public Bool HasTeletext { get; private set; }
			public Bool HasMPEG4TimedText { get; private set; }
			public Bool HasXDS { get; private set; }

			public ReportData(ILogger logger){
				Logger = logger;

				StreamMode = "Unknown";
			}

			public void setValue(string key,string value){
				switch (key) {
				case "Stream Mode":
					StreamMode = value;
					break;
				case "DVB Subtitles":
					HasDVB = GetBoolFromString (value);
					break;
				case "Teletext":
					HasTeletext = GetBoolFromString (value);
					break;
				case "EIA-608":
					Has608 = GetBoolFromString (value);
					break;
				case "CEA-708":
					Has708 = GetBoolFromString (value);
					break;
				case "MPEG-4 Timed Text":
					HasMPEG4TimedText = GetBoolFromString (value);
					break;
				case "XDS":
					HasXDS = GetBoolFromString (value);
					break;
				default:
					break;
				}
			}

			public string CreateRow (string path)
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
					,path,StreamMode,Has608,Has708,HasTeletext,HasDVB,HasMPEG4TimedText,HasXDS);
			}

			public Bool GetBoolFromString(string value){
				value = value.Trim ();
				switch (value) {
				case "Yes":
					return Bool.Yes;
				case "No":
					return Bool.No;
				default:
					Logger.Debug (value + " cannot be converted to a Bool value");
					return Bool.Undefined;
				}
			}
		}
	}
}


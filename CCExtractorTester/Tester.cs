using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace CCExtractorTester
{
	// TODO: add option to change Comparer, and add option for Logger.
	public class Tester
	{
		public List<TestEntry> Entries { get; private set; } 
		private IProgressReportable ProgressReporter { get; set; }
		private IFileComparable Comparer { get; set; }
		private ConfigurationSettings Config { get; set; }
		private ILogger Logger { get; set; }
		private IPerformanceLogger PerformanceLogger { get; set; }

		public Tester(ConfigurationSettings cfg,ILogger logger){
			Entries = new List<TestEntry> ();
			ProgressReporter = new NullProgressReporter ();
			Config = cfg;
			Logger = logger;
			LoadComparer ();
			LoadPerformanceLogger ();
		}

		public Tester (ConfigurationSettings cfg,ILogger logger,string xmlFile) : this(cfg,logger)
		{
			if (!String.IsNullOrEmpty(xmlFile)) {
				loadAndParseXML (xmlFile);
			}
		}

		void LoadComparer ()
		{
			switch (Config.GetAppSetting ("Comparer")) {
			case "diff":
				Comparer = new DiffLinuxComparer ();
				break;
			case "diffplexreduced":
				Comparer = new DiffToolComparer (true);
				break;
			case "diffplex":
				// Fall-through to default.
			default:
				Comparer = new DiffToolComparer (false);
				break;
			}

		}

		void LoadPerformanceLogger ()
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32NT:
				PerformanceLogger = new WindowsPerformanceCounters ();
				break;			
			default:
				PerformanceLogger = new NullLogger ();
				break;
			}
			
		}

		public void SaveEntriesToXML(string fileName){
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (@"<?xml version=""1.0"" encoding=""UTF-8""?><tests></tests>");
			XmlNode root = doc.DocumentElement;
			foreach (TestEntry te in Entries) {
				XmlNode t = doc.CreateElement ("test");

				XmlNode sample = doc.CreateElement ("sample");
				sample.InnerText = te.TestFile;
				t.AppendChild (sample);
				XmlNode cmd = doc.CreateElement ("cmd");
				cmd.InnerText = te.Command;
				t.AppendChild (cmd);
				XmlNode result = doc.CreateElement ("result");
				result.InnerText = te.ResultFile;
				t.AppendChild (result);
				root.AppendChild (t);
			}
			doc.Save (fileName);
		}

		void loadAndParseXML (string xmlFile)
		{
			if (File.Exists (xmlFile)) {
				ValidateXML (xmlFile);
				XmlDocument doc = new XmlDocument ();
				using(FileStream fs = new FileStream(xmlFile,FileMode.Open)){
					doc.Load (fs);
					foreach (XmlNode node in doc.SelectNodes("//test")) {
						XmlNode sampleFile = node.SelectSingleNode ("sample");
						XmlNode command = node.SelectSingleNode ("cmd");
						XmlNode resultFile = node.SelectSingleNode ("result");
						Entries.Add(new TestEntry(ConvertFolderDelimiters(sampleFile.InnerText),command.InnerText,ConvertFolderDelimiters(resultFile.InnerText)));
					}
				}
				return;
			}
			throw new InvalidDataException ("File does not exist");
		}

		void ValidateXML (string xml)
		{
			using (StringReader sr = new StringReader (Resources.tests)) {
				XmlReader r = XmlReader.Create (sr);
				XmlReaderSettings settings = new XmlReaderSettings ();
				settings.Schemas.Add (null, r);
				settings.ValidationType = ValidationType.Schema;
				settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler (settings_ValidationEventHandler);
				using (FileStream fs = new FileStream (xml, FileMode.Open)) {
					var reader = XmlReader.Create (fs, settings);
				}
			}
		}

		string ConvertFolderDelimiters (string path)
		{
			char env = '\\';
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Win32NT:
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.WinCE:
				env = '/';
					break;			
				default:					
					break;
			}
			return path.Replace (env, Path.DirectorySeparatorChar);
		}

		void settings_ValidationEventHandler (object sender, System.Xml.Schema.ValidationEventArgs e)
		{
			throw new InvalidDataException ("XML File is not formatted correctly");
		}

		public void RunTests(){
			String cce = Config.GetAppSetting ("CCExtractorLocation");
			if (!File.Exists (cce)) {
				throw new InvalidOperationException ("CCExtractor location ("+cce+") is not a valid file/executable");
			}
			String sourceFolder = Config.GetAppSetting ("SampleFolder");
			if (!Directory.Exists (sourceFolder)) {
				throw new InvalidOperationException ("Sample folder does not exist!");
			}

			String location = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			location = location.Remove (location.LastIndexOf (Path.DirectorySeparatorChar));
			if(!Directory.Exists(Path.Combine(location,"tmpFiles"))){
				Directory.CreateDirectory (Path.Combine (location, "tmpFiles"));
			}

			int i = 1;
			int total = Entries.Count;

			ProcessStartInfo psi = new ProcessStartInfo(cce);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;

			foreach (TestEntry te in Entries) {
				ProgressReporter.showProgressMessage (String.Format ("Starting with entry {0} of {1}", i, total));

				string sampleFile = Path.Combine (sourceFolder, te.TestFile);
				string producedFile = Path.Combine (location,"tmpFiles", te.ResultFile.Substring (te.ResultFile.LastIndexOf (Path.DirectorySeparatorChar) + 1));
				string expectedResultFile = Path.Combine (Config.GetAppSetting ("CorrectResultFolder"), te.ResultFile);

				psi.Arguments = te.Command + String.Format(@" --no_progress_bar -o ""{0}"" ""{1}""  ",producedFile,sampleFile);
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
				if (p.ExitCode == 0) {
					try {
						Comparer.CompareAndAddToResult (
							new CompareData(){ 
								ProducedFile = producedFile,
								CorrectFile = expectedResultFile,
								SampleFile = sampleFile,
								Command = te.Command,
								RunTime=(p.ExitTime-p.StartTime)
							});
					} catch(Exception e){
						Logger.Error (e);
					}
				}
				ProgressReporter.showProgressMessage (String.Format ("Finished entry {0} with exit code: {1}", i,p.ExitCode));
				i++;
			}
			File.WriteAllText(
				Path.Combine (Config.GetAppSetting("ReportFolder"),Comparer.GetReportFileName()),
				Comparer.GetResult (new ResultData(){CCExtractorVersion = cce+" "+DateTime.Now.ToShortDateString()}));
		}

		void processOutput (object sender, DataReceivedEventArgs e)
		{
			Logger.Debug (e.Data);
		}

		void processError (object sender, DataReceivedEventArgs e)
		{
			if (!String.IsNullOrEmpty (e.Data)) {
				Logger.Error (e.Data);
			}
		}

		public void SetProgressReporter (IProgressReportable progressReporter)
		{
			ProgressReporter = progressReporter;
		}

		class NullProgressReporter : IProgressReportable {
			#region IProgressReportable implementation
			public void showProgressMessage (string message)
			{
				// do nothing.
			}

			public void showProgramMessage (string message)
			{
				// do nothing
			}
			#endregion
		}

		class NullLogger : IPerformanceLogger {
			#region IPerformanceLogger implementation

			public void SetUp (ILogger logger, Process p)
			{
				// do nothing
			}

			public void DebugValue ()
			{
				// do nothing
			}

			public void DebugStats ()
			{
				// do nothing
			}
			#endregion
		}
	}
}
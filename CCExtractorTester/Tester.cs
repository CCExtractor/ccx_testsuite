using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

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

		public Tester(ConfigurationSettings cfg,ILogger logger){
			Entries = new List<TestEntry> ();
			ProgressReporter = new NullProgressReporter ();
			Config = cfg;
			Logger = logger;
			LoadComparer ();
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
			case "diffplex":
				// Fall-through to default.
			default:
				Comparer = new DiffToolComparer ();
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
				doc.Load (xmlFile);
				foreach (XmlNode node in doc.SelectNodes("//test")) {
					XmlNode sampleFile = node.SelectSingleNode ("sample");
					XmlNode command = node.SelectSingleNode ("cmd");
					XmlNode resultFile = node.SelectSingleNode ("result");
					Entries.Add(new TestEntry(sampleFile.InnerText,command.InnerText,resultFile.InnerText));
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

				var reader = XmlReader.Create (xml, settings);
			}
		}

		void settings_ValidationEventHandler (object sender, System.Xml.Schema.ValidationEventArgs e)
		{
			throw new InvalidDataException ("XML File is not formatted correctly");
		}

		public void RunTests(){
			String cce = Config.GetAppSetting ("CCExtractorLocation");
			if (!File.Exists (cce)) {
				throw new InvalidOperationException ("CCExtractor location is not a valid file/executable");
			}
			String sourceFolder = Config.GetAppSetting ("SampleFolder");
			if (!Directory.Exists (sourceFolder)) {
				throw new InvalidOperationException ("Sample folder does not exist!");
			}

			String location = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			location = location.Remove (location.LastIndexOf (Path.DirectorySeparatorChar));

			int i = 1;
			int total = Entries.Count;

			ProcessStartInfo psi = new ProcessStartInfo(cce);
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;

			PerformanceCounter ram, cpu;

			foreach (TestEntry te in Entries) {
				ProgressReporter.showProgressMessage (String.Format ("Starting with entry {0} of {1}", i, total));
				string sampleFile = Path.Combine (sourceFolder, te.TestFile);
				string producedFile = Path.Combine (location, "tmp_" + te.ResultFile.Substring (te.ResultFile.LastIndexOf (Path.DirectorySeparatorChar) + 1));
				string expectedResultFile = Path.Combine (Config.GetAppSetting ("CorrectResultFolder"), te.ResultFile);
				psi.Arguments = te.Command + String.Format(@" -o ""{0}"" ""{1}""  ",producedFile,sampleFile);
				Logger.Debug ("Passed arguments: "+psi.Arguments);
				Process p = new Process ();
				p.StartInfo = psi;
				p.ErrorDataReceived += processError;
				p.OutputDataReceived += processOutput;
				p.Start ();

				ram = cpu = null;
				try {
					ram = new PerformanceCounter ("Process", "Working Set", p.ProcessName);
					cpu = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
				} catch(Exception e){
					Logger.Error (e);
				}
				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();
				while (!p.HasExited) {
					if(ram != null && cpu != null){
						Logger.Debug(String.Format("Process usage stats: {0} MB of ram, {1} % CPU",(ram.NextValue()/1024/1024),cpu.NextValue()));
					}
					Thread.Sleep (500);
				}
				Logger.Debug ("Process Exited. Exit code: " + p.ExitCode);
				Logger.Debug(String.Format("Process data: handles opened: {0}, peak paged mem: {1}, peak virtual mem: {2}, peak mem: {3}, privileged cpu time: {4}, total cpu time: {5}",p.HandleCount,p.PeakPagedMemorySize64,p.PeakVirtualMemorySize64,p.PeakWorkingSet64,p.PrivilegedProcessorTime,p.TotalProcessorTime));
				if (p.ExitCode == 0) {
					Comparer.CompareAndAddToResult (
						new CompareData(){ 
							CorrectFile=expectedResultFile,
							ProducedFile=producedFile,
							RunTime=(p.ExitTime-p.StartTime)
						});
				}
				ProgressReporter.showProgressMessage (String.Format ("Finished entry {0} with exit code: {1}", i,p.ExitCode));
				i++;
			}
			File.WriteAllText(Path.Combine (Config.GetAppSetting("ReportFolder"),Comparer.GetReportFileName()),Comparer.GetResult ());
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
	}
}
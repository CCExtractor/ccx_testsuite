using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CCExtractorTester
{
	public class Tester
	{
		public List<TestEntry> Entries { get; private set; } 
		private IProgressReportable Reporter { get; set; }
		private IFileComparable Comparer { get; set; }
		private ConfigurationSettings Config { get; set; }

		public Tester(ConfigurationSettings cfg){
			Entries = new List<TestEntry> ();
			Reporter = new NullReporter ();
			Comparer = new NullComparer ();
			Config = cfg;

		}

		public Tester (ConfigurationSettings cfg,string xmlFile) : this(cfg)
		{
			if (!String.IsNullOrEmpty(xmlFile)) {
				loadAndParseXML (xmlFile);
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

			foreach (TestEntry te in Entries) {
				Reporter.showProgressMessage (String.Format ("Starting with entry {0} of {1}", i, total));
				psi.Arguments = te.Command + String.Format(@" -o ""{0}"" ""{1}""  ",Path.Combine(location,"tmp_"+te.ResultFile.Substring(te.ResultFile.LastIndexOf(Path.DirectorySeparatorChar)+1)),Path.Combine(sourceFolder,te.TestFile));
				MainClass.Logger.Info (psi.Arguments);
				Process p = new Process ();
				p.StartInfo = psi;
				p.ErrorDataReceived += processError;
				p.OutputDataReceived += processOutput;
				p.Start ();
				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();
				while (!p.HasExited) {
					Thread.Sleep (1000);
				}
				if (p.ExitCode == 0) {
					// TODO: implement report generating through calling the Comparer.
				}
				Reporter.showProgressMessage (String.Format ("Finished entry {0} with exit code: {1}", i,p.ExitCode));
				i++;
			}
		}

		void processOutput (object sender, DataReceivedEventArgs e)
		{
			MainClass.Logger.Info (e.Data);
		}

		void processError (object sender, DataReceivedEventArgs e)
		{
			MainClass.Logger.Error (e.Data);
		}

		public void SetReporter (IProgressReportable reporter)
		{
			Reporter = reporter;
		}

		class NullReporter : IProgressReportable {
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

		class NullComparer : IFileComparable {
			#region IFileComparable implementation

			public string Compare (string fileLocation1, string fileLocation2)
			{
				return "";
			}

			#endregion
		}
	}
}


using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace CCExtractorTester
{
	public class Tester
	{
		private List<TestEntry> Entries { get; set; } 

		public Tester (string xml)
		{
			Entries = new List<TestEntry> ();
			loadAndParseXML (xml);
		}

		void loadAndParseXML (string xml)
		{
			if (File.Exists (xml)) {
				// TODO: create xml validation (xsd) and validate against it first.
				XmlDocument doc = new XmlDocument ();
				doc.Load (xml);
				foreach (XmlNode node in doc.SelectNodes("//test")) {
					XmlNode sampleFile = node.SelectSingleNode ("sample");
					XmlNode command = node.SelectSingleNode ("cmd");
					XmlNode resultFile = node.SelectSingleNode ("result");
					Entries.Add(new TestEntry(sampleFile.Value,command.Value,resultFile.Value));
				}
			}
			throw new InvalidDataException ("File does not exist");
		}

		public void RunTests(){
			// TODO: finish
		}
	}
}


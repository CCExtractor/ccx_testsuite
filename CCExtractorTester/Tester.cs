﻿using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace CCExtractorTester
{
	public class Tester
	{
		public List<TestEntry> Entries { get; private set; } 

		public Tester(){
			Entries = new List<TestEntry> ();
		}

		public Tester (string xml) : this()
		{
			if (!String.IsNullOrEmpty(xml)) {
				loadAndParseXML (xml);
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

		void loadAndParseXML (string xml)
		{
			if (File.Exists (xml)) {
				ValidateXML (xml);
				XmlDocument doc = new XmlDocument ();
				doc.Load (xml);
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
			// TODO: finish
		}
	}
}


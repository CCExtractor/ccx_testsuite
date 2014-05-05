using System;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;

namespace CCExtractorTester
{
	public class ConfigurationSettings
	{
		private bool IsConfiguration = true;

		private Configuration ConfigManager { get; set; }

		private String ConfigXMLLocation { get; set; }
		private XmlDocument ConfigXML { get; set; }

		private Dictionary<string,string> Settings { get; set; }

		public ConfigurationSettings ()
		{
			ConfigManager = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
			LoadSettings ();
		}

		public ConfigurationSettings(XmlDocument xml,String xmlSaveLocation){
			IsConfiguration = false;
			ConfigXML = xml;
			LoadSettings ();
		}

		private void LoadSettings ()
		{
			Settings = new Dictionary<string, string> ();
			if (IsConfiguration) {
				foreach (KeyValueConfigurationElement kce in ConfigManager.AppSettings.Settings) {
					Settings.Add (kce.Key, kce.Value);
				}
			} else {
				foreach (XmlNode n in ConfigXML.SelectNodes("configuration/appSettings/add")) {
					Settings.Add(n.Attributes["key"].Value,n.Attributes["value"].Value);
				}
			}
		}

		public bool IsAppConfigOK ()
		{
			return (
				!String.IsNullOrEmpty (GetAppSetting("ReportFolder")) &&
				!String.IsNullOrEmpty (GetAppSetting("SampleFolder")) &&
				!String.IsNullOrEmpty (GetAppSetting("CorrectResultFolder")) &&
				!String.IsNullOrEmpty (GetAppSetting("CCExtractorLocation"))
			);			
		}

		public string GetAppSetting (string key)
		{
			if (Settings.ContainsKey (key)) {
				return Settings [key];
			}
			return "";
		}

		public void SetAppSetting(string key, string value){
			if (Settings.ContainsKey (key)) {
				Settings [key] = value;
				if (IsConfiguration) {
					ConfigManager.AppSettings.Settings [key].Value = value;
				} else {
					XmlNode add = ConfigXML.SelectSingleNode ("configuration/appSettings/add[@key='" + key + "']");
					add.Attributes ["value"].Value = value;
				}
			} else {
				Settings.Add (key, value);
				if (IsConfiguration) {
					ConfigManager.AppSettings.Settings.Add (key, value);
				} else {
					XmlAttribute keyAttr = ConfigXML.CreateAttribute ("key");
					keyAttr.Value = key;
					XmlAttribute valueAttr = ConfigXML.CreateAttribute ("value");
					valueAttr.Value = value;
					XmlNode add = ConfigXML.CreateElement ("add");
					add.Attributes.Append (keyAttr);
					add.Attributes.Append (valueAttr);
					XmlNode appSettings = ConfigXML.SelectSingleNode ("configuration/appSettings");
					appSettings.AppendChild (add);
				}
			}
		}

		public void SaveConfiguration(){
			if (IsConfiguration) {
				ConfigManager.Save (ConfigurationSaveMode.Minimal);
			} else {
				ConfigXML.Save (ConfigXMLLocation);
			}
		}
	}
}
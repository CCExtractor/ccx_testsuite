using System;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;

namespace CCExtractorTester
{
    /// <summary>
    /// Configuration settings that are used throughout the application.
    /// </summary>
    public class ConfigurationSettings
    {
        /// <summary>
        /// Indicates use of the app.config file.
        /// </summary>
        private bool IsAppConfiguration = true;

        /// <summary>
        /// Gets or sets the config manager.
        /// </summary>
        /// <value>The config manager.</value>
        private Configuration ConfigManager { get; set; }

        /// <summary>
        /// Gets or sets the config XML.
        /// </summary>
        /// <value>The config XML.</value>
        private XmlDocument ConfigXML { get; set; }

        /// <summary>
        /// Keeps a list of all the settings.
        /// </summary>
        /// <value>The settings.</value>
        private Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCExtractorTester.ConfigurationSettings"/> class.
        /// </summary>
        public ConfigurationSettings()
        {
            ConfigManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            LoadSettings();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CCExtractorTester.ConfigurationSettings"/> class.
        /// </summary>
        /// <param name="xml">The XML document that contains the settings.</param>
        /// <param name="xmlSaveLocation">The location to save a modified config xml to.</param>
        public ConfigurationSettings(XmlDocument xml, String xmlSaveLocation)
        {
            IsAppConfiguration = false;
            ConfigXML = xml;
            LoadSettings();
        }

        /// <summary>
        /// Loads/parses the settings.
        /// </summary>
        private void LoadSettings()
        {
            Settings = new Dictionary<string, string>();
            if (IsAppConfiguration)
            {
                foreach (KeyValueConfigurationElement kce in ConfigManager.AppSettings.Settings)
                {
                    Settings.Add(kce.Key, kce.Value);
                }
            }
            else
            {
                foreach (XmlNode n in ConfigXML.SelectNodes("configuration/appSettings/add"))
                {
                    Settings.Add(n.Attributes["key"].Value, n.Attributes["value"].Value);
                }
            }
        }

        /// <summary>
        /// Determines if the app config is OK or not.
        /// </summary>
        /// <returns><c>true</c> if the ReportFolder,SampleFolder,CorrectResultFolder and the CCExtractorLocation are set; otherwise, <c>false</c>.</returns>
        public bool IsAppConfigOK()
        {
            return (
                !String.IsNullOrEmpty(GetAppSetting("ReportFolder")) &&
                !String.IsNullOrEmpty(GetAppSetting("SampleFolder")) &&
                !String.IsNullOrEmpty(GetAppSetting("CorrectResultFolder")) &&
                !String.IsNullOrEmpty(GetAppSetting("CCExtractorLocation"))
            );
        }

        /// <summary>
        /// Gets a specific app setting.
        /// </summary>
        /// <returns>The app setting.</returns>
        /// <param name="key">The key for the app setting that needs to be retrieved.</param>
        public string GetAppSetting(string key)
        {
            if (Settings.ContainsKey(key))
            {
                return Settings[key];
            }
            return "";
        }

        /// <summary>
        /// Sets an app setting.
        /// </summary>
        /// <param name="key">The key to set the app setting for.</param>
        /// <param name="value">The new value to set for the given key.</param>
        public void SetAppSetting(string key, string value)
        {
            if (Settings.ContainsKey(key))
            {
                Settings[key] = value;
                if (IsAppConfiguration)
                {
                    ConfigManager.AppSettings.Settings[key].Value = value;
                }
                else
                {
                    XmlNode add = ConfigXML.SelectSingleNode("configuration/appSettings/add[@key='" + key + "']");
                    add.Attributes["value"].Value = value;
                }
            }
            else
            {
                Settings.Add(key, value);
                if (IsAppConfiguration)
                {
                    ConfigManager.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    XmlAttribute keyAttr = ConfigXML.CreateAttribute("key");
                    keyAttr.Value = key;
                    XmlAttribute valueAttr = ConfigXML.CreateAttribute("value");
                    valueAttr.Value = value;
                    XmlNode add = ConfigXML.CreateElement("add");
                    add.Attributes.Append(keyAttr);
                    add.Attributes.Append(valueAttr);
                    XmlNode appSettings = ConfigXML.SelectSingleNode("configuration/appSettings");
                    appSettings.AppendChild(add);
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using testGenerations.Properties;

namespace testGenerations
{
    public class TestGenerations
    {
        /// <summary>
        /// Indicates a first generation file (tests).
        /// </summary>
        public const string XML_FIRST_GENERATION = "first_gen";
        /// <summary>
        /// Indicates a second generation file (multitests).
        /// </summary>
        public const string XML_SECOND_GENERATION = "second_gen";
        /// <summary>
        /// Indicates a third generation file (testsuite).
        /// </summary>
        public const string XML_THIRD_GENERATION = "third_gen";

        /// <summary>
        /// Validates the given XML at the location against our scheme.
        /// </summary>
        /// <param name="xmlFileName">The location of the XML file to validate.</param>
        /// <returns>A string that indicates the generation of the XML file.</returns>
        public static string ValidateXML(string xmlFileName)
        {
            Dictionary<string, string> schemes = new Dictionary<string, string>() {
                { XML_THIRD_GENERATION, Resources.testsuite },
                { XML_SECOND_GENERATION, Resources.multitest },
                { XML_FIRST_GENERATION, Resources.tests }
            };

            foreach (KeyValuePair<string, string> kvp in schemes)
            {
                try
                {
                    ValidateAgainstSchema(xmlFileName, kvp.Value);
                    return kvp.Key;
                }
                catch (XmlSchemaValidationException)
                {
                    continue;
                }
            }

            throw new InvalidDataException(string.Format("Given XML ({0}) is not a valid file for the possible formats.", xmlFileName));
        }

        /// <summary>
        /// Validates the XML against a given schema.
        /// </summary>
        /// <param name="xmlFileName">Xml file name.</param>
        /// <param name="xmlSchema">Xml schema.</param>
        private static void ValidateAgainstSchema(string xmlFileName, string xmlSchema)
        {
            using (StringReader sr = new StringReader(xmlSchema))
            {
                XmlReader r = XmlReader.Create(sr);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, r);
                settings.ValidationType = ValidationType.Schema;
                using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
                {
                    var reader = XmlReader.Create(fs, settings);
                    while (reader.Read())
                    {
                        // Nothing in here, just need to read out the entire file in a loop.
                    }
                }
            }
        }
    }
}

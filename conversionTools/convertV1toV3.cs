using System;
using System.IO;
using System.Xml;
using testGenerations;

namespace conversionTools
{
    /// <summary>
    /// Converts a given v1 file into a v3 file, in a simple way.
    /// </summary>
    class convertV1toV3
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please specify an input file!");
                Console.WriteLine("Usage: convertV1toV3 {input}");
                return;
            }
            // Check if file exists
            string firstGen = args[0];
            if (!File.Exists(firstGen))
            {
                Console.WriteLine("{0} is not a valid file!", firstGen);
                return;
            }
            // Check if given file is a valid first generation xml
            try
            {
                string generation = TestGenerations.ValidateXML(firstGen);
                if (generation != TestGenerations.XML_FIRST_GENERATION)
                {
                    Console.WriteLine("{0} is a valid test file, but not of the first generation!", firstGen);
                    return;
                }
            }
            catch (InvalidDataException)
            {
                Console.WriteLine("{0} is not a valid first generation xml test file!", firstGen);
                return;
            }
            // Parse & convert
            XmlDocument doc = new XmlDocument();
            XmlDocument result = new XmlDocument();
            result.LoadXml(@"<?xml version=""1.0"" encoding=""UTF - 8""?><tests></tests>");
            XmlNode tests = result.SelectSingleNode("tests");
            using (FileStream fs = new FileStream(firstGen, FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                XmlNodeList testNodes = doc.SelectNodes("//test");
                foreach (XmlNode node in testNodes)
                {
                    // Load current data
                    XmlNode sampleFile = node.SelectSingleNode("sample");
                    XmlNode command = node.SelectSingleNode("cmd");
                    XmlNode resultFile = node.SelectSingleNode("result");
                    // Store in new format
                    XmlElement entry = result.CreateElement("entry");
                    XmlElement entryCommand = result.CreateElement("command");
                    entryCommand.AppendChild(result.CreateTextNode(command.InnerText));
                    entry.AppendChild(entryCommand);
                    XmlElement input = result.CreateElement("input");
                    input.SetAttribute("type", "file");
                    input.AppendChild(result.CreateTextNode(sampleFile.InnerText));
                    entry.AppendChild(input);
                    XmlElement output = result.CreateElement("output");
                    output.AppendChild(result.CreateTextNode("file"));
                    entry.AppendChild(output);
                    XmlElement compare = result.CreateElement("compare");
                    XmlElement file = result.CreateElement("file");
                    file.SetAttribute("ignore", "false");
                    XmlElement correct = result.CreateElement("correct");
                    correct.AppendChild(result.CreateTextNode(resultFile.InnerText));
                    file.AppendChild(correct);
                    compare.AppendChild(file);
                    entry.AppendChild(compare);
                    tests.AppendChild(entry);
                }
                // Save generated output file
                using(XmlWriter xw = XmlWriter.Create(firstGen.Substring(0, firstGen.Length - 4) + "_3rd.xml"))
                {
                    result.WriteTo(xw);
                }
            }
        }
    }
}

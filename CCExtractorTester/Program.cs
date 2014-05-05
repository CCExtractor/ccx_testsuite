using System;
using System.IO;
using System.Xml;
using Gtk;

namespace CCExtractorTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length > 0) {
				ConfigurationSettings config = new ConfigurationSettings ();
				if (args.Length > 1) {
					if (File.Exists (args [1]) && args [1].EndsWith (".xml")) {
						XmlDocument doc = new XmlDocument ();
						doc.Load (args [1]);
						config = new ConfigurationSettings (doc, args [1]);
					} else {
						Console.WriteLine ("[ERROR] The second argument provided either doesn't exist or is not an .xml file");
						WriteConsoleSampleUsage ();
						Console.WriteLine ("[INFO] The default appsettings will be used (if they exist)");
					}
				}
				if (!config.IsAppConfigOK ()) {
					Console.WriteLine("[ERROR] Please edit the config file and try again");
					return;
				}
				if (File.Exists (args [0]) && args [0].EndsWith (".xml")) {
					Tester t = new Tester (config,args [0]);
					t.SetReporter (new ConsoleReporter());
					t.RunTests ();
				} else {
					Console.WriteLine ("[ERROR] The first argument provided is either doesn't exist or is not an .xml file.");
					WriteConsoleSampleUsage ();
				}
			} else {
				Application.Init ();
				ConfigurationSettings config = new ConfigurationSettings ();
				using (FileChooserDialog filechooser =
					new FileChooserDialog (
						"Choose the file to load the config from or press cancel to use the app.config",
						null,FileChooserAction.Open,
						"Cancel", ResponseType.Cancel,
						"Select", ResponseType.Accept)
				) {
					if (filechooser.Run () == (int)ResponseType.Accept) {
						XmlDocument doc = new XmlDocument ();
						doc.Load (filechooser.Filename);
						config = new ConfigurationSettings(doc,filechooser.Filename);
					}
					filechooser.Destroy ();
				}

				MainWindow win = new MainWindow (config);
				win.Show ();
				Application.Run ();
			}
		}

		static void WriteConsoleSampleUsage ()
		{
			Console.WriteLine ();
			Console.WriteLine ("[INFO] Sample usage of this program is: ");
			Console.WriteLine ("[INFO] ccextractortester path/to/tests-file.xml path/to/options.xml");
			Console.WriteLine ("[INFO] where the .xml file is a valid CCEXtractorTexter tests file");
			Console.WriteLine ();
		}

		class ConsoleReporter : IProgressReportable {
			#region IProgressReportable implementation

			public void showProgressMessage (string message)
			{
				Console.WriteLine ("[PROGRESS] "+message);
			}

			#endregion
		}
	}
}
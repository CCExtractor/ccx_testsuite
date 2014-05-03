using System;
using Gtk;
using System.Configuration;
using System.IO;

namespace CCExtractorTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length > 0) {
				if (!ConfigWindow.IsAppConfigOK ()) {
					Console.WriteLine("Please edit the app.config file and try again");
					return;
				}
				if (File.Exists (args [0]) && args [0].EndsWith (".xml")) {
					Tester t = new Tester (args [0]);
					t.SetReporter (new ConsoleReporter());
					t.RunTests ();
				} else {
					Console.WriteLine ("[ERROR] The argument provided is either doesn't exist or is not an .xml file.");
					Console.WriteLine ("[INFO] Sample usage of this program is: ");
					Console.WriteLine ("[INFO] ccextractortester path/to/xml-file.xml");
					Console.WriteLine ("[INFO] where the .xml file is a valid CCEXtractorTexter tests file");
				}
			} else {
				Application.Init ();
				MainWindow win = new MainWindow ();
				win.Show ();
				Application.Run ();
			}
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
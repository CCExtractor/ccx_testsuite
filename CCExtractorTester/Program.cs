using System;
using Gtk;
using System.Configuration;

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
				// TODO: check arguments.
				// TODO: run program
			} else {
				Application.Init ();
				MainWindow win = new MainWindow ();
				win.Show ();
				Application.Run ();
			}
		}
	}
}
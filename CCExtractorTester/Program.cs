using System;
using Gtk;
using System.Configuration;

namespace CCExtractorTester
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			bool console = args.Length > 0;
			if (console) {
				try {
					VerifyAppConfig ();

					// TODO: run program;
				} catch (InvalidProgramException e) {
					Console.WriteLine (e.Message);
				}
			} else {
				Application.Init ();
				try {
					VerifyAppConfig ();
				} catch (InvalidProgramException) {
					RunSetUpScreen ();
				}
				MainWindow win = new MainWindow ();
				win.Show ();
				Application.Run ();
			}
		}

		static void VerifyAppConfig ()
		{
			if (String.IsNullOrEmpty (ConfigurationManager.AppSettings ["ReportFolder"]) ||
			   String.IsNullOrEmpty (ConfigurationManager.AppSettings ["SampleFolder"]) ||
			   String.IsNullOrEmpty (ConfigurationManager.AppSettings ["CorrectResultFolder"]) ||
			   String.IsNullOrEmpty (ConfigurationManager.AppSettings ["CCExtractorLocation"])) 
			{
				throw new InvalidProgramException ("Please edit the app.config file and try again");
			}
		}

		static void RunSetUpScreen ()
		{
			ConfigWindow cw = new ConfigWindow ();
			cw.Show ();
			Application.Run ();
		}
	}
}

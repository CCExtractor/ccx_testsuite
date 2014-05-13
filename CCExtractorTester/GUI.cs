using System;
using Gtk;
using System.Xml;

namespace CCExtractorTester
{
	public class GUI
	{
		public static void Run(ILogger logger){
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
					logger.Debug ("Loading provided configuration: "+filechooser.Filename);
					XmlDocument doc = new XmlDocument ();
					doc.Load (filechooser.Filename);
					config = new ConfigurationSettings(doc,filechooser.Filename);
				}
				filechooser.Destroy ();
			}
			MainWindow win = new MainWindow (config,logger);
			win.Show ();
			Application.Run ();
		}
	}
}


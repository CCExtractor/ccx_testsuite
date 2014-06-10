using System;
using Gtk;
using System.Xml;

namespace CCExtractorTester
{
	/// <summary>
	/// The class that handles the start-up of a GUI. Separated from the Program class to avoid issues with GTK when only wanting to use the console.
	/// </summary>
	public class GUI
	{
		/// <summary>
		/// Runs the GUI with a specified logger.
		/// </summary>
		/// <param name="logger">The logger to pass on to the GUI.</param>
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
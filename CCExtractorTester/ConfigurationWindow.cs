using System;

namespace CCExtractorTester
{
	public partial class ConfigurationWindow : Gtk.Window
	{
		public ConfigurationWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}
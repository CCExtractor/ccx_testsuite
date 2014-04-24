using System;
using Gtk;

namespace CCExtractorTester
{
	public partial class MainWindow: Gtk.Window
	{
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			InitComponents ();
		}

		void InitComponents ()
		{
			if (!ConfigWindow.IsAppConfigOK ()) {
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "The configuration is incomplete. Please configure the application using the 'Configure Application' menu item");
				md.Run ();
				md.Destroy ();
			}
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnConfigureApplicationActionActivated (object sender, EventArgs e)
		{
			ConfigWindow cw = new ConfigWindow();
			cw.Show();
		}

		protected void OnRunTestsActionActivated (object sender, EventArgs e)
		{
			// TODO: run tests
		}

		protected void OnOpenActionActivated (object sender, EventArgs e)
		{
			// TODO: open file
		}

		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			// TODO: save file
		}

		protected void OnQuitActionActivated (object sender, EventArgs e)
		{
			Application.Quit();
		}
	}		
}
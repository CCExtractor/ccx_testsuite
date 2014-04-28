using System;
using Gtk;

namespace CCExtractorTester
{
	public partial class MainWindow: Gtk.Window
	{
		private Tester TestClass { get; set; }
		private ListStore Store { get; set; }

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
			if (!String.IsNullOrEmpty (ConfigWindow.GetAppSetting ("DefaultTestFile"))) {
				TestClass = new Tester (ConfigWindow.GetAppSetting ("DefaultTestFile"));
			} else {
				TestClass = new Tester ();
			}
			InitTreeview ();
			AddEntries ();
		}

		void InitTreeview ()
		{
			CellRendererText crt = new CellRendererText ();

			tree.AppendColumn ("Sample file", crt, "text", 0);
			tree.AppendColumn ("Commandline arguments", crt, "text", 1);
			tree.AppendColumn ("Result file", crt, "text", 2);

			tree.Selection.Mode = SelectionMode.Multiple;
		}

		void AddEntries ()
		{
			Store = new ListStore(typeof(string),typeof(string),typeof(string));
			tree.Model = Store;
			foreach (TestEntry te in TestClass.Entries) {
				Store.AppendValues (te.TestFile, te.Command, te.ResultFile);
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
			TestClass.RunTests ();
		}

		protected void OnOpenActionActivated (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the test XML file to open",
					this,
					FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				filechooser.AddFilter (GetTestFilter());
				if (filechooser.Run () == (int)ResponseType.Accept) {
					try {
						TestClass = new Tester(filechooser.Filename);
						AddEntries();
					} catch(Exception){
					}
				}
				filechooser.Destroy ();
			}
		}

		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			// TODO: save file
		}

		protected void OnQuitActionActivated (object sender, EventArgs e)
		{
			Application.Quit();
		}

		protected void OnBtnRemoveRowClicked (object sender, EventArgs e)
		{
			foreach(TreePath tp in tree.Selection.GetSelectedRows()){
				TreeIter iter;
				tree.Model.GetIter(out iter,tp);
				Store.Remove (ref iter);
			}
		}

		protected void OnBtnEditRowClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnBtnAddRowClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		public FileFilter GetTestFilter ()
		{
			FileFilter f = new FileFilter ();
			f.Name = "XML files";
			f.AddPattern ("*.xml");
			return f;
		}
	}
}
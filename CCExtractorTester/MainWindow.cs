using System;
using Gtk;
using System.Collections.Generic;

namespace CCExtractorTester
{
	public partial class MainWindow: Gtk.Window, ICalleable, IProgressReportable
	{
		private Tester TestClass { get; set; }
		private ListStore Store { get; set; }
		private ConfigurationSettings Config { get; set; }

		public MainWindow (ConfigurationSettings cs) : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			Config = cs;
			InitComponents ();
		}

		void InitComponents ()
		{
			if (!Config.IsAppConfigOK ()) {
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "The configuration is incomplete. Please configure the application using the 'Configure Application' menu item");
				md.Run ();
				md.Destroy ();
			}
			if (!String.IsNullOrEmpty (Config.GetAppSetting ("DefaultTestFile"))) {
				TestClass = new Tester (Config,Config.GetAppSetting("DefaultTestFile"));
			} else {
				TestClass = new Tester (Config);
			}
			TestClass.SetReporter (this);
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
			ConfigWindow cw = new ConfigWindow(Config);
			cw.Show();
		}


		protected void OnRunTestsActionActivated (object sender, EventArgs e)
		{
			SyncEntries ();
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
						TestClass = new Tester(Config,filechooser.Filename);
						AddEntries();
					} catch(Exception){
					}
				}
				filechooser.Destroy ();
			}
		}

		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the test XML file to save the current entries in",
					this,
					FileChooserAction.Save,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				if (filechooser.Run () == (int)ResponseType.Accept) {
					SyncEntries ();
					TestClass.SaveEntriesToXML (filechooser.Filename);
				}
				filechooser.Destroy ();
			}
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
			foreach (TreePath tp in tree.Selection.GetSelectedRows()) {
				TreeIter iter;
				tree.Model.GetIter (out iter, tp);			
				AddEntryDialog aed = new AddEntryDialog (Config,(string)Store.GetValue (iter, 0), (string)Store.GetValue (iter, 1), (string)Store.GetValue (iter, 2),this,iter);
				aed.Show ();
			}
		}

		protected void OnBtnAddRowClicked (object sender, EventArgs e)
		{
			AddEntryDialog aed = new AddEntryDialog (Config,this,null);
			aed.Show ();
		}

		private void SyncEntries ()
		{
			TestClass.Entries.Clear ();
			foreach (object[] row in Store) {
				TestEntry t = new TestEntry ((string)row [0],(string) row [1],(string) row [2]);
				TestClass.Entries.Add (t);
			}
		}

		public FileFilter GetTestFilter ()
		{
			FileFilter f = new FileFilter ();
			f.Name = "XML files";
			f.AddPattern ("*.xml");
			return f;
		}

		#region ICalleable implementation

		public void Call (Dictionary<string, object> callbackValues)
		{
			if (callbackValues.ContainsKey ("key")) {
				if (callbackValues ["key"] != null) {
					TreeIter ti = (TreeIter)callbackValues ["key"];
					Store.SetValues (ti, callbackValues ["sample"], callbackValues ["cmd"], callbackValues ["result"]);
				} else {
					Store.AppendValues (callbackValues ["sample"], callbackValues ["cmd"], callbackValues ["result"]);
				}
			}
		}

		#endregion

		#region IProgressReportable implementation

		public void showProgressMessage (string message)
		{
			statusbar1.Pop (statusbar1.GetContextId ("Tester"));
			statusbar1.Push (statusbar1.GetContextId ("Tester"), message);
		}

		#endregion
	}
}
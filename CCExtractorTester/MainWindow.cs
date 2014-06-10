using System;
using Gtk;
using System.Collections.Generic;

namespace CCExtractorTester
{
	/// <summary>
	/// Main window of the application.
	/// </summary>
	public partial class MainWindow: Gtk.Window, ICalleable, IProgressReportable
	{
		/// <summary>
		/// Gets or sets the test class.
		/// </summary>
		/// <value>The test class.</value>
		private Tester TestClass { get; set; }
		/// <summary>
		/// Gets or sets the store.
		/// </summary>
		/// <value>The store.</value>
		private ListStore Store { get; set; }
		/// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		private ConfigurationSettings Config { get; set; }
		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
		private ILogger Logger { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.MainWindow"/> class.
		/// </summary>
		/// <param name="cs">The configuration settings to use.</param>
		/// <param name="logger">The logger to use.</param>
		public MainWindow (ConfigurationSettings cs,ILogger logger) : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			Config = cs;
			Logger = logger;
			InitComponents ();
		}

		/// <summary>
		/// Inits the components.
		/// </summary>
		void InitComponents ()
		{
			if (!Config.IsAppConfigOK ()) {
				Logger.Warn ("Configuration incomplete!");
				MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "The configuration is incomplete. Please configure the application using the 'Configure Application' menu item");
				md.Run ();
				md.Destroy ();
			}
			if (!String.IsNullOrEmpty (Config.GetAppSetting ("DefaultTestFile"))) {
				try {
				TestClass = new Tester (Config,Logger,Config.GetAppSetting("DefaultTestFile"));
				} catch(Exception e){
					Logger.Error ("Could not find default Test file!");
					Logger.Error (e);
					TestClass = new Tester (Config, Logger);
				}
			} else {
				TestClass = new Tester (Config,Logger);
			}
			TestClass.SetProgressReporter (this);
			InitTreeview ();
			AddEntries ();
		}

		/// <summary>
		/// Inits the treeview that holds the test entries.
		/// </summary>
		void InitTreeview ()
		{
			CellRendererText crt = new CellRendererText ();

			tree.AppendColumn ("Sample file", crt, "text", 0);
			tree.AppendColumn ("Commandline arguments", crt, "text", 1);
			tree.AppendColumn ("Result file", crt, "text", 2);

			tree.Selection.Mode = SelectionMode.Multiple;
		}

		/// <summary>
		/// Adds the entries to the treeview.
		/// </summary>
		void AddEntries ()
		{
			Store = new ListStore(typeof(string),typeof(string),typeof(string));
			tree.Model = Store;
			foreach (TestEntry te in TestClass.Entries) {
				Store.AppendValues (te.TestFile, te.Command, te.ResultFile);
			}
		}

		/// <summary>
		/// Raises the delete event event. The application will quit.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="a">The alpha component.</param>
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		/// <summary>
		/// Raises the configure application action activated event. Will open the window to edit the configuration
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnConfigureApplicationActionActivated (object sender, EventArgs e)
		{
			ConfigWindow cw = new ConfigWindow(Config);
			cw.Show();
		}

		/// <summary>
		/// Raises the run tests action activated event. Runs all tests after syncing the entries with the tester.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnRunTestsActionActivated (object sender, EventArgs e)
		{
			SyncEntries ();
			try {
				TestClass.RunTests ();
			} catch(Exception ex){
				Logger.Error (ex);
				ShowErrorDialog (ex.Message);
			}
		}

		/// <summary>
		/// Raises the open action activated event. Allows the user to choose a file and open it if one is chosen.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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
				filechooser.AddFilter (GetXMLFileFilter());
				if (filechooser.Run () == (int)ResponseType.Accept) {
					try {
						TestClass = new Tester(Config,Logger,filechooser.Filename);
						AddEntries();
					} catch(Exception ex){
						Logger.Error(ex);
						ShowErrorDialog (ex.Message);
					}
				}
				filechooser.Destroy ();
			}
		}

		/// <summary>
		/// Raises the save action activated event. Saves
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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
					try {
					SyncEntries ();
					TestClass.SaveEntriesToXML (filechooser.Filename);
					} catch(Exception ex){
						Logger.Error(ex);
						ShowErrorDialog (ex.Message);
					}
				}
				filechooser.Destroy ();
			}
		}

		/// <summary>
		/// Raises the quit action activated event. Quits the application
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnQuitActionActivated (object sender, EventArgs e)
		{
			Application.Quit();
		}

		/// <summary>
		/// Raises the button remove row clicked event. Removes the selected rows from the data store.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnRemoveRowClicked (object sender, EventArgs e)
		{
			TreePath[] treePaths = tree.Selection.GetSelectedRows ();
			for (int i = treePaths.Length-1; i >=0; i--) {
				TreeIter iter;
				tree.Model.GetIter(out iter,treePaths[i]);
				Store.Remove (ref iter);
			}
		}

		/// <summary>
		/// Raises the button edit row clicked event. Opens an edit dialog for each selected row.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnEditRowClicked (object sender, EventArgs e)
		{
			foreach (TreePath tp in tree.Selection.GetSelectedRows()) {
				TreeIter iter;
				tree.Model.GetIter (out iter, tp);			
				AddEntryDialog aed = new AddEntryDialog (Config,(string)Store.GetValue (iter, 0), (string)Store.GetValue (iter, 1), (string)Store.GetValue (iter, 2),this,iter);
				aed.Show ();
			}
		}

		/// <summary>
		/// Raises the button add row clicked event. Opens a dialog to add a new row.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnAddRowClicked (object sender, EventArgs e)
		{
			AddEntryDialog aed = new AddEntryDialog (Config,this,null);
			aed.Show ();
		}

		/// <summary>
		/// Syncs the entries with the Tester instance.
		/// </summary>
		private void SyncEntries ()
		{
			TestClass.Entries.Clear ();
			foreach (object[] row in Store) {
				TestEntry t = new TestEntry ((string)row [0],(string) row [1],(string) row [2]);
				TestClass.Entries.Add (t);
			}
		}

		/// <summary>
		/// Gets the filter for XML files.
		/// </summary>
		/// <returns>The filter for XML files</returns>
		public FileFilter GetXMLFileFilter ()
		{
			FileFilter f = new FileFilter ();
			f.Name = "XML files";
			f.AddPattern ("*.xml");
			return f;
		}

		/// <summary>
		/// Shows an error dialog.
		/// </summary>
		/// <param name="message">The message to show.</param>
		void ShowErrorDialog (string message)
		{
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "An exception occured: "+message+"\n\nMore information in the log file");
			md.Run ();
			md.Destroy ();
		}

		/// <summary>
		/// Shows a warning dialog.
		/// </summary>
		/// <param name="message">The message to show.</param>
		public void ShowWarningDialog (string message)
		{
			MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, message);
			md.Run ();
			md.Destroy ();
		}

		#region ICalleable implementation
		/// <summary>
		/// Call the instance back with the specified callbackValues.
		/// </summary>
		/// <param name="callbackValues">Callback values.</param>
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
		/// <summary>
		/// Shows the progress message.
		/// </summary>
		/// <param name="message">The progress message to show.</param>
		public void showProgressMessage (string message)
		{
			statusbar1.Pop (statusbar1.GetContextId ("Tester"));
			statusbar1.Push (statusbar1.GetContextId ("Tester"), message);
		}
		#endregion
	}
}
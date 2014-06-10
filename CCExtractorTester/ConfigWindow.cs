using System;
using Gtk;
using System.Configuration;

namespace CCExtractorTester
{
	/// <summary>
	/// Config window class. Allows user to change config settings using GUI.
	/// </summary>
	public partial class ConfigWindow : Gtk.Window
	{
		/// <summary>
		/// Gets or sets the config object.
		/// </summary>
		/// <value>The config.</value>
		private ConfigurationSettings Config { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.ConfigWindow"/> class.
		/// </summary>
		/// <param name="cs">The configuration settings to use.</param>
		public ConfigWindow (ConfigurationSettings cs) : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Config = cs;
			this.InitComponents ();
		}

		/// <summary>
		/// Initializes the components.
		/// </summary>
		void InitComponents ()
		{
			this.txtReport.Text = Config.GetAppSetting ("ReportFolder");
			this.txtSample.Text = Config.GetAppSetting ("SampleFolder");
			this.txtResult.Text = Config.GetAppSetting ("CorrectResultFolder");
			this.txtCCExtractor.Text = Config.GetAppSetting ("CCExtractorLocation");
			this.txtDefault.Text = Config.GetAppSetting ("DefaultTestFile");
		}

		/// <summary>
		/// Raises the button save clicked event. Saves the configuration, and if the config is ok, closes the window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			Config.SaveConfiguration ();
			if (Config.IsAppConfigOK ()) {
				this.Destroy ();
			}
		}

		/// <summary>
		/// Lets the user select a file, and saves the result to a given setting.
		/// </summary>
		/// <param name="isFile"><c>true</c> for a file, <c>false</c> for a directory</param>
		/// <param name="field">the field that will be updated upon change.</param>
		/// <param name="key">the config key setting.</param>
		protected void SelectFileAndSave (bool isFile,Entry field,string key)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file or folder for this setting",
					this,isFile ? FileChooserAction.Open : FileChooserAction.SelectFolder,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				if (filechooser.Run () == (int)ResponseType.Accept) {
					field.Text = filechooser.Filename;
					Config.SetAppSetting (key, filechooser.Filename);
				}
				filechooser.Destroy ();
			}
		}

		/// <summary>
		/// Raises the button report clicked event. Used to modifiy the report folder.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnReportClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtReport,"ReportFolder");
		}
		/// <summary>
		/// Raises the button sample clicked event. Modifies the sample folder.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnSampleClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtSample,"SampleFolder");
		}
		/// <summary>
		/// Raises the button result clicked event. Modifies the correct result folder
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnResultClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtResult,"CorrectResultFolder");
		}
		/// <summary>
		/// Raises the button CC extractor clicked event. Modifies the path to the CCExtractor executable.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnCCExtractorClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (true,txtCCExtractor,"CCExtractorLocation");
		}
		/// <summary>
		/// Raises the button default clicked event. Modifies the default test file.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnDefaultClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (true,txtDefault,"DefaultTestFile");
		}

		/// <summary>
		/// Raises the delete event event. Closes this window.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="a">The alpha component.</param>
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Destroy ();
			a.RetVal = true;
		}
	}
}
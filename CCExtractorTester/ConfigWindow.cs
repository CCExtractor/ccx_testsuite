using System;
using Gtk;
using System.Configuration;

namespace CCExtractorTester
{
	public partial class ConfigWindow : Gtk.Window
	{
		private ConfigurationSettings Config { get; set; }

		public ConfigWindow (ConfigurationSettings cs) : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Config = cs;
			this.InitComponents ();
		}

		void InitComponents ()
		{
			this.txtReport.Text = Config.GetAppSetting ("ReportFolder");
			this.txtSample.Text = Config.GetAppSetting ("SampleFolder");
			this.txtResult.Text = Config.GetAppSetting ("CorrectResultFolder");
			this.txtCCExtractor.Text = Config.GetAppSetting ("CCExtractorLocation");
			this.txtDefault.Text = Config.GetAppSetting ("DefaultTestFile");
		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			Config.SaveConfiguration ();
			if (Config.IsAppConfigOK ()) {
				this.Destroy ();
			}
		}

		protected void SelectFileAndSave (bool isFile,Entry lbl,string key)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file or folder for this setting",
					this,isFile ? FileChooserAction.Open : FileChooserAction.SelectFolder,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				if (filechooser.Run () == (int)ResponseType.Accept) {
					lbl.Text = filechooser.Filename;
					Config.SetAppSetting (key, filechooser.Filename);
				}
				filechooser.Destroy ();
			}
		}

		protected void OnBtnReportClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtReport,"ReportFolder");
		}

		protected void OnBtnSampleClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtSample,"SampleFolder");
		}

		protected void OnBtnResultClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (false,txtResult,"CorrectResultFolder");
		}

		protected void OnBtnCCExtractorClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (true,txtCCExtractor,"CCExtractorLocation");
		}

		protected void OnBtnDefaultClicked (object sender, EventArgs e)
		{
			SelectFileAndSave (true,txtDefault,"DefaultTestFile");
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Destroy ();
			a.RetVal = true;
		}
	}
}
using System;
using Gtk;
using System.Configuration;

namespace CCExtractorTester
{
	public partial class ConfigWindow : Gtk.Window
	{
		Configuration cm;

		public ConfigWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			cm = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);
			this.Build ();
			this.InitComponents ();
		}

		void InitComponents ()
		{
			this.txtReport.Text = GetAppSetting ("ReportFolder");
			this.txtSample.Text = GetAppSetting ("SampleFolder");
			this.txtResult.Text = GetAppSetting ("CorrectResultFolder");
			this.txtCCExtractor.Text = GetAppSetting ("CCExtractorLocation");
			this.txtDefault.Text = GetAppSetting ("DefaultTestFile");
		}

		string GetAppSetting (string key)
		{
			String value = cm.AppSettings.Settings[key].Value;
			if(String.IsNullOrEmpty(value)){
				return "";
			}
			return value;
		}

		void SetAppSetting(string key, string value){
			cm.AppSettings.Settings[key].Value = value;
		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			cm.Save (ConfigurationSaveMode.Minimal);
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
					SetAppSetting (key, filechooser.Filename);
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
	}
}
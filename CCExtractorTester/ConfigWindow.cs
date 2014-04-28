using System;
using Gtk;
using System.Configuration;

namespace CCExtractorTester
{
	public partial class ConfigWindow : Gtk.Window
	{
		static Configuration cm = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.None);

		public ConfigWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			this.InitComponents ();
		}

		public static bool IsAppConfigOK ()
		{
			return (
				!String.IsNullOrEmpty (cm.AppSettings.Settings ["ReportFolder"].Value) &&
				!String.IsNullOrEmpty (cm.AppSettings.Settings ["SampleFolder"].Value) &&
				!String.IsNullOrEmpty (cm.AppSettings.Settings ["CorrectResultFolder"].Value) &&
				!String.IsNullOrEmpty (cm.AppSettings.Settings ["CCExtractorLocation"].Value)
			);			
		}

		public static string GetAppSetting (string key)
		{
			String value = cm.AppSettings.Settings[key].Value;
			if(String.IsNullOrEmpty(value)){
				return "";
			}
			return value;
		}

		public static void SetAppSetting(string key, string value){
			cm.AppSettings.Settings[key].Value = value;
		}

		void InitComponents ()
		{
			this.txtReport.Text = GetAppSetting ("ReportFolder");
			this.txtSample.Text = GetAppSetting ("SampleFolder");
			this.txtResult.Text = GetAppSetting ("CorrectResultFolder");
			this.txtCCExtractor.Text = GetAppSetting ("CCExtractorLocation");
			this.txtDefault.Text = GetAppSetting ("DefaultTestFile");
		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			cm.Save (ConfigurationSaveMode.Minimal);
			if (IsAppConfigOK ()) {
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

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Destroy ();
			a.RetVal = true;
		}
	}
}
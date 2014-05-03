using System;
using System.Collections.Generic;
using Gtk;

namespace CCExtractorTester
{
	public partial class AddEntryDialog : Gtk.Dialog
	{
		private ICalleable Callback { get; set; }
		private Dictionary<string,object> CallbackValues { get; set; }

		public AddEntryDialog (ICalleable callback,object key)
		{
			Callback = callback;
			CallbackValues = new Dictionary<string, object> ();
			CallbackValues.Add ("key", key);
			this.Build ();
		}

		public AddEntryDialog (string sample,string cmd,string result,ICalleable callback,object key) : this(callback,key)
		{
			txtSample.Text = sample;
			txtCmd.Text = cmd;
			txtResult.Text = result;
		}

		protected void OnBntSelectClicked (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file to use as sample for this entry",
					this,FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				filechooser.SetCurrentFolder(ConfigWindow.GetAppSetting ("SampleFolder"));
				if (filechooser.Run () == (int)ResponseType.Accept) {
					txtSample.Text = filechooser.Filename.Replace(ConfigWindow.GetAppSetting ("SampleFolder"),"");
				}
				filechooser.Destroy ();
			}
		}

		protected void OnBtnSelectClicked (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file to use as result for this entry",
					this,FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				filechooser.SetCurrentFolder(ConfigWindow.GetAppSetting ("CorrectResultFolder"));
				if (filechooser.Run () == (int)ResponseType.Accept) {
					txtResult.Text = filechooser.Filename.Replace(ConfigWindow.GetAppSetting ("CorrectResultFolder"),"");
				}
				filechooser.Destroy ();
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			CallbackValues.Add ("sample", txtSample.Text);
			CallbackValues.Add ("cmd", txtCmd.Text);
			CallbackValues.Add ("result", txtResult.Text);
			Callback.Call (CallbackValues);
			this.Destroy ();
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}


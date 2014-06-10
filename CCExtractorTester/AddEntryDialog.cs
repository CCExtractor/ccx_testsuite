using System;
using System.Collections.Generic;
using Gtk;

namespace CCExtractorTester
{
	/// <summary>
	/// The class holding the dialog to add or edit an entry from the test list.
	/// </summary>
	public partial class AddEntryDialog : Gtk.Dialog
	{
		/// <summary>
		/// Gets or sets the callback.
		/// </summary>
		/// <value>The callback.</value>
		private ICalleable Callback { get; set; }
		/// <summary>
		/// Gets or sets the callback values.
		/// </summary>
		/// <value>The callback values.</value>
		private Dictionary<string,object> CallbackValues { get; set; }
		/// <summary>
		/// Gets or sets the configuration object to use.
		/// </summary>
		/// <value>The config.</value>
		private ConfigurationSettings Config { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.AddEntryDialog"/> class.
		/// </summary>
		/// <param name="cs">The configuration setting to use.</param>
		/// <param name="callback">The object that will be called back.</param>
		/// <param name="key">The key that is used.</param>
		public AddEntryDialog (ConfigurationSettings cs,ICalleable callback,object key)
		{
			Callback = callback;
			Config = cs;
			CallbackValues = new Dictionary<string, object> ();
			CallbackValues.Add ("key", key);
			this.Build ();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.AddEntryDialog"/> class.
		/// </summary>
		/// <param name="cs">The configuration setting to use.</param>
		/// <param name="sample">The sample file location, relative to the folder setting for it.</param>
		/// <param name="cmd">The command line arguments</param>
		/// <param name="result">The result file location, relative to the folder settin for it.</param>
		/// <param name="callback">The object to call back.</param>
		/// <param name="key">The key.</param>
		public AddEntryDialog (ConfigurationSettings cs,string sample,string cmd,string result,ICalleable callback,object key) : this(cs,callback,key)
		{
			txtSample.Text = sample;
			txtCmd.Text = cmd;
			txtResult.Text = result;
		}

		/// <summary>
		/// Raises the bnt select clicked event. Shows a file chooser, and saves the result in the sample.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBntSelectClicked (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file to use as sample for this entry",
					this,FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				filechooser.SetCurrentFolder(Config.GetAppSetting ("SampleFolder"));
				if (filechooser.Run () == (int)ResponseType.Accept) {
					txtSample.Text = filechooser.Filename.Replace(Config.GetAppSetting ("SampleFolder"),"").Substring(1);
				}
				filechooser.Destroy ();
			}
		}

		/// <summary>
		/// Raises the button select clicked event. Shows a file chooser and saves the result to the correct result field.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnSelectClicked (object sender, EventArgs e)
		{
			using (FileChooserDialog filechooser =
				new FileChooserDialog (
					"Choose the file to use as result for this entry",
					this,FileChooserAction.Open,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept)
			) {
				filechooser.SetCurrentFolder(Config.GetAppSetting ("CorrectResultFolder"));
				if (filechooser.Run () == (int)ResponseType.Accept) {
					txtResult.Text = filechooser.Filename.Replace(Config.GetAppSetting ("CorrectResultFolder"),"").Substring(1);
				}
				filechooser.Destroy ();
			}
		}

		/// <summary>
		/// Raises the button ok clicked event. Adds the entered fields to the callback object, and performs the callback. Closes the dialog afterwards.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			CallbackValues.Add ("sample", txtSample.Text);
			CallbackValues.Add ("cmd", txtCmd.Text);
			CallbackValues.Add ("result", txtResult.Text);
			Callback.Call (CallbackValues);
			this.Destroy ();
		}

		/// <summary>
		/// Raises the button cancel clicked event. Closes the dialog
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}
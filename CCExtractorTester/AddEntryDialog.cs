using System;

namespace CCExtractorTester
{
	public partial class AddEntryDialog : Gtk.Dialog
	{
		public AddEntryDialog ()
		{
			this.Build ();
		}

		public AddEntryDialog (string sample,string cmd,string result) : this()
		{
			txtSample.Text = sample;
			txtCmd.Text = cmd;
			txtResult.Text = result;
		}

		protected void OnBntSelectClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnBtnSelectClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}


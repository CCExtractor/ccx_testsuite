using System;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace CCExtractorTester
{
	public class DiffLinuxComparer : IFileComparable
	{
		private StringBuilder Builder { get; set; }

		public DiffLinuxComparer ()
		{
			Builder = new StringBuilder ();
		}

		#region IFileComparable implementation

		public void CompareAndAddToResult (string fileLocation1, string fileLocation2, string extraHTML = "")
		{
			Builder.AppendLine (extraHTML);
			ProcessStartInfo psi = new ProcessStartInfo("diff");
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;

			psi.Arguments = String.Format(@"-y ""{0}"" ""{1}""",fileLocation1,fileLocation2);
			Process p = new Process ();
			p.StartInfo = psi;
			p.ErrorDataReceived += processError;
			p.OutputDataReceived += processOutput;
			p.Start ();
			p.BeginOutputReadLine ();
			p.BeginErrorReadLine ();
			while (!p.HasExited) {
				Thread.Sleep (1000);
			}
		}

		public string GetResult ()
		{
			return Builder.ToString ();
		}

		public string GetReportFileName ()
		{
			return "Report_" + DateTime.Now.ToFileTime () + ".txt";
		}

		#endregion

		void processError (object sender, DataReceivedEventArgs e)
		{
			Builder.AppendLine (e.Data);
		}

		void processOutput (object sender, DataReceivedEventArgs e)
		{
			Builder.AppendLine (e.Data);
		}
	}
}


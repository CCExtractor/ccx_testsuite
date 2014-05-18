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

		public void CompareAndAddToResult (CompareData data)
		{
			Builder.AppendLine ("Time needed for this entry: "+data.RunTime.ToString());
			Builder.AppendLine ("Used command: " + data.Command);
			Builder.AppendLine ("Sample file: " + data.SampleFile);
			ProcessStartInfo psi = new ProcessStartInfo("diff");
			psi.UseShellExecute = false;
			psi.RedirectStandardError = true;
			psi.RedirectStandardOutput = true;
			psi.CreateNoWindow = true;

			psi.Arguments = String.Format(@"-y ""{0}"" ""{1}""",data.CorrectFile,data.ProducedFile);
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

		public string GetResult (ResultData data)
		{
			return "Report generated for version "+data.CCExtractorVersion+"\n"+Builder.ToString ();
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


using System;
using System.IO;

namespace CCExtractorTester
{
	public class FileLogger : Ilogger
	{
		StreamWriter Writer { get; set; }


		public FileLogger ()
		{
			Writer = File.CreateText ("Log-Run-"+DateTime.Now.ToFileTime()+".txt");
		}

		#region Ilogger implementation

		public void Info (string message)
		{
			Writer.WriteLine ("[INFO] " + message);
			Writer.Flush ();
		}

		public void Warn (string message)
		{
			Writer.WriteLine ("[WARNING] " + message);
			Writer.Flush ();
		}

		public void Error (string message)
		{
			Writer.WriteLine ("[ERROR] " + message);
			Writer.Flush ();
		}

		public void Error (Exception e)
		{
			Error (e.Message);
			Error ("Stacktrace:");
			Error (e.StackTrace);
		}

		public void Debug (string message)
		{
			Writer.WriteLine ("[DEBUG] " + message);
			Writer.Flush ();
		}

		#endregion
	}
}
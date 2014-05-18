using System;
using System.IO;

namespace CCExtractorTester
{
	public class FileLogger : ILogger
	{
		StreamWriter Writer { get; set; }
		bool IsDebug { get; set; }


		public FileLogger ()
		{
			Writer = File.CreateText ("Log-Run-"+DateTime.Now.ToFileTime()+".txt");
			IsDebug = false;
		}

		#region Ilogger implementation

		public void ActivateDebug ()
		{
			IsDebug = true;
		}

		public virtual void Info (string message)
		{
			Writer.WriteLine ("[INFO] " + message);
			Writer.Flush ();
		}

		public virtual void Warn (string message)
		{
			Writer.WriteLine ("[WARNING] " + message);
			Writer.Flush ();
		}

		public virtual void Error (string message)
		{
			Writer.WriteLine ("[ERROR] " + message);
			Writer.Flush ();
		}

		public virtual void Error (Exception e)
		{
			Error (e.Message);
			Error ("Stacktrace:");
			Error (e.StackTrace);
		}

		public virtual void Debug (string message)
		{
			if (IsDebug) {
				Writer.WriteLine ("[DEBUG] " + message);
				Writer.Flush ();
			}
		}

		#endregion
	}
}
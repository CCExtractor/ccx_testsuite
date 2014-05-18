using System;

namespace CCExtractorTester
{
	public class ConsoleFileLogger : ILogger
	{
		public ILogger Logger { get; private set; }
		private bool IsDebug { get; set; }

		public ConsoleFileLogger (): base()
		{
			IsDebug = false;
			Logger = new FileLogger ();
		}

		public void ActivateDebug ()
		{
			IsDebug = true;
			Logger.ActivateDebug ();
		}

		public void Info (string message)
		{
			Console.WriteLine ("[INFO] " + message);
			Logger.Info (message);
		}

		public void Warn (string message)
		{
			Console.WriteLine ("[WARN] " + message);
			Logger.Warn (message);
		}

		public void Error (string message)
		{
			Console.WriteLine ("[ERROR] " + message);
			Logger.Error (message);
		}

		public void Error (Exception e)
		{
			Console.WriteLine ("[ERROR] " + e.Message);
			Logger.Error (e);
		}

		public void Debug (string message)
		{
			Logger.Debug (message);
		}
	}
}
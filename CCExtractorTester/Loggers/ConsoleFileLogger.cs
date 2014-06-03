using System;

namespace CCExtractorTester
{
	/// <summary>
	/// Console + file logger. Logs to both Console and a file.
	/// </summary>
	public class ConsoleFileLogger : ILogger
	{
		/// <summary>
		/// Gets the internal Logger. All the log requests will be passed on to this one.
		/// </summary>
		/// <value>The logger.</value>
		public ILogger Logger { get; private set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance logging debug messages.
		/// </summary>
		/// <value><c>true</c> if this instance will print debug messages; otherwise, <c>false</c>.</value>
		private bool IsDebug { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.ConsoleFileLogger"/> class.
		/// </summary>
		public ConsoleFileLogger (): base()
		{
			IsDebug = false;
			Logger = new FileLogger ();
		}

		#region Ilogger implementation
		/// <summary>
		/// Activates the debug mode. If it's not activated, no debug messages will be logged.
		/// </summary>
		public void ActivateDebug ()
		{
			IsDebug = true;
			Logger.ActivateDebug ();
		}

		/// <summary>
		/// Logs a specific message on the Info level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Info (string message)
		{
			Console.WriteLine ("[INFO] " + message);
			Logger.Info (message);
		}

		/// <summary>
		/// Logs a specific message on the Warn level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Warn (string message)
		{
			Console.WriteLine ("[WARN] " + message);
			Logger.Warn (message);
		}

		/// <summary>
		/// Logs a specific message on the Error level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Error (string message)
		{
			Console.WriteLine ("[ERROR] " + message);
			Logger.Error (message);
		}

		/// <summary>
		/// Logs a specific message on the Error level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		/// <param name="e">E.</param>
		public void Error (Exception e)
		{
			Console.WriteLine ("[ERROR] " + e.Message);
			Logger.Error (e);
		}

		/// <summary>
		/// Logs a specific message on the Debug level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Debug (string message)
		{
			Logger.Debug (message);
		}
		#endregion
	}
}
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
		public ILogger FileLogger { get; private set; }
		/// <summary>
		/// Gets or sets a value indicating whether this instance logging debug messages.
		/// </summary>
		/// <value><c>true</c> if this instance will print debug messages; otherwise, <c>false</c>.</value>
		private bool IsDebug { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.ConsoleFileLogger"/> class.
		/// </summary>
		/// <param name="logDirectory">The directory to create the log file in.</param>
		public ConsoleFileLogger (String logDirectory): base()
		{
			IsDebug = false;
			FileLogger = new FileLogger (logDirectory);
		}

		#region Ilogger implementation
		/// <summary>
		/// Activates the debug mode. If it's not activated, no debug messages will be logged.
		/// </summary>
		public void ActivateDebug ()
		{
			IsDebug = true;
			FileLogger.ActivateDebug ();
		}

		/// <summary>
		/// Logs a specific message on the Info level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Info (string message)
		{
			Console.WriteLine ("[INFO] " + message);
			FileLogger.Info (message);
		}

		/// <summary>
		/// Logs a specific message on the Warn level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Warn (string message)
		{
			Console.WriteLine ("[WARN] " + message);
			FileLogger.Warn (message);
		}

		/// <summary>
		/// Logs a specific message on the Error level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Error (string message)
		{
			Console.WriteLine ("[ERROR] " + message);
			FileLogger.Error (message);
		}

		/// <summary>
		/// Logs a specific message on the Error level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		/// <param name="e">E.</param>
		public void Error (Exception e)
		{
			Console.WriteLine ("[ERROR] " + e.Message);
			FileLogger.Error (e);
		}

		/// <summary>
		/// Logs a specific message on the Debug level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		public void Debug (string message)
		{
			FileLogger.Debug (message);
		}
		#endregion
	}
}
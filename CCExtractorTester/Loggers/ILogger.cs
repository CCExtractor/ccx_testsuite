using System;

namespace CCExtractorTester
{
	/// <summary>
	/// ILogger, the interface for Logging classes.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs a specific message on the Info level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		void Info(string message);
		/// <summary>
		/// Logs a specific message on the Warn level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		void Warn(string message);
		/// <summary>
		/// Logs a specific message on the Error level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		void Error(string message);
		/// <summary>
		/// Logs a specific exception on the Error level.
		/// </summary>
		/// <param name="e">The exception to log</param>
		void Error(Exception e);
		/// <summary>
		/// Logs a specific message on the Debug level.
		/// </summary>
		/// <param name="message">A message to log.</param>
		void Debug(string message);
		/// <summary>
		/// Activates the debug mode. If it's not activated, no debug messages will be logged.
		/// </summary>
		void ActivateDebug();
	}
}
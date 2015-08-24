using System;
using System.IO;

namespace CCExtractorTester
{
    /// <summary>
    /// A class that logs to a file.
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Gets or sets the an instance of StreamWriter.
        /// </summary>
        /// <value>The writer.</value>
        StreamWriter Writer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance logging debug messages.
        /// </summary>
        /// <value><c>true</c> if this instance will print debug messages; otherwise, <c>false</c>.</value>
        bool IsDebug { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CCExtractorTester.FileLogger"/> class.
        /// </summary>
        /// <param name="logDirectory">The directory to create the log file in.</param>
        public FileLogger(String logDirectory)
        {
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            Writer = File.CreateText(Path.Combine(logDirectory, "Log-Run-" + DateTime.Now.ToFileTime() + ".txt"));
            IsDebug = false;
        }

        #region Ilogger implementation
        /// <summary>
        /// Activates the debug mode. If it's not activated, no debug messages will be logged.
        /// </summary>
        public void ActivateDebug()
        {
            IsDebug = true;
        }
        /// <summary>
        /// Logs a specific message on the Info level.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public virtual void Info(string message)
        {
            Writer.WriteLine("[INFO] " + message);
            Writer.Flush();
        }
        /// <summary>
        /// Logs a specific message on the Warn level.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public virtual void Warn(string message)
        {
            Writer.WriteLine("[WARNING] " + message);
            Writer.Flush();
        }
        /// <summary>
        /// Logs a specific message on the Error level.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public virtual void Error(string message)
        {
            Writer.WriteLine("[ERROR] " + message);
            Writer.Flush();
        }
        /// <summary>
        /// Logs a specific message on the Error level.
        /// </summary>
        /// <param name="message">A message to log.</param>
        /// <param name="e">E.</param>
        public virtual void Error(Exception e)
        {
            Error(e.Message);
            Error("Stacktrace:");
            Error(e.StackTrace);
        }
        /// <summary>
        /// Logs a specific message on the Debug level.
        /// </summary>
        /// <param name="message">A message to log.</param>
        public virtual void Debug(string message)
        {
            if (IsDebug)
            {
                Writer.WriteLine("[DEBUG] " + message);
                Writer.Flush();
            }
        }
        #endregion
    }
}
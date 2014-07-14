using System;
using System.IO;

namespace CCExtractorTester
{
	public class Reporter
	{
		/// <summary>
		/// Gets or sets the progress reporter that will be used.
		/// </summary>
		/// <value>The progress reporter.</value>
		private IProgressReportable ProgressReporter { get; set; }
		/// <summary>
		/// Gets or sets the configuration instance that will be used.
		/// </summary>
		/// <value>The config.</value>
		private ConfigurationSettings Config { get; set; }
		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
		private ILogger Logger { get; set; }
		/// <summary>
		/// Gets or sets the matrix.
		/// </summary>
		/// <value>The matrix.</value>
		private string Matrix { get; set; }

		public Reporter (ConfigurationSettings config, ILogger logger, string matrix)
		{
			Config = config;
			Logger = logger;
			Matrix = matrix;		
		}

		public void SetProgressReporter (IProgressReportable progressReporter)
		{
			ProgressReporter = progressReporter;
		}

		public void GenerateMatrix ()
		{
			String cce = Config.GetAppSetting ("CCExtractorLocation");
			if (!File.Exists (cce)) {
				throw new InvalidOperationException ("CCExtractor location ("+cce+") is not a valid file/executable");
			}
			Runner r = new Runner (cce, Logger,NullPerformanceLogger.Instance);
			// TODO: finish
		}
	}
}


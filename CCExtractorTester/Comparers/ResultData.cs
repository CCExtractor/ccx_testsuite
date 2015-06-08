using System;

namespace CCExtractorTester
{
	/// <summary>
	/// A class for holding some data that should be in the report.
	/// </summary>
	public class ResultData
	{
		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		public String FileName { get; set; }
		/// <summary>
		/// Gets or sets the CC extractor version.
		/// </summary>
		/// <value>The CC extractor version.</value>
		public String CCExtractorVersion { get; set; }
		/// <summary>
		/// Gets or sets the start time.
		/// </summary>
		/// <value>The start time.</value>
		public DateTime StartTime { get; set; }
	}
}
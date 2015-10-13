using System;

namespace CCExtractorTester.Comparers
{
	/// <summary>
	/// A class holding all the info that should be passed from the tester to the report generator.
	/// </summary>
	public class CompareData
	{
		/// <summary>
		/// Gets or sets the correct file.
		/// </summary>
		public string CorrectFile { get; set; }
		/// <summary>
		/// Gets or sets the produced file.
		/// </summary>
		public string ProducedFile { get; set; }
		/// <summary>
		/// Gets or sets the sample file.
		/// </summary>
		public string SampleFile { get; set; }
		/// <summary>
		/// Gets or sets the commands used.
		/// </summary>
		public string Command { get; set; }
		/// <summary>
		/// Gets or sets the run time of the entry.
		/// </summary>
		public TimeSpan RunTime { get; set; }
		/// <summary>
		/// Gets or sets the exit code.
		/// </summary>
		public int ExitCode { get; set; }
        /// <summary>
        /// Gets or sets the dummy bool.
        /// </summary>
        public bool Dummy { get; set; }
    }
}
namespace CCExtractorTester
{
    /// <summary>
    /// This enum defines how the test suite will behave during it's lifetime.
    /// </summary>
    enum RunType
    {
        /// <summary>
        /// Generates reports and stores them on the executing machine.
        /// </summary>
        Report,
        /// <summary>
        /// Passes the results back to a server for further storage & processing.
        /// </summary>
        Server,
        /// <summary>
        /// Generates a matrix report for passed in folder using CCExtractor's built in report functionality.
        /// </summary>
        Matrix
    }
}
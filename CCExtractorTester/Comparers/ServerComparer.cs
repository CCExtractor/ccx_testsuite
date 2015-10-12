using System.Collections.Specialized;
using System.Net;
using CCExtractorTester.Analyzers;

namespace CCExtractorTester.Comparers
{
    /// <summary>
    /// Class for sending data back to a server instead of doing the comparison locally.
    /// </summary>
    public class ServerComparer : IFileComparable
    {
        /// <summary>
        /// The URL where the class will send status updates to.
        /// </summary>
        private string reportUrl;

        private static string userAgent = "CCExctractor Automated Test Suite";

        /// <summary>
        /// Generates an new instance of this class.
        /// </summary>
        /// <param name="reportUrl">The URL where the class will send status updates to.</param>
        public ServerComparer(string reportUrl)
        {
            this.reportUrl = reportUrl;
        }

        #region IFileComparable implementation
        /// <summary>
        /// Compares the files provided in the data and add to an internal result.
        /// </summary>
        /// <param name="data">The data for this entry.</param>
        public void CompareAndAddToResult(CompareData data)
        {
            string hash = Hasher.getFileHash(data.ProducedFile);

            // Check for equality by hash
            if (!Hasher.filesAreEqual(data.CorrectFile, data.ProducedFile))
            {
                // Upload result
                using (var wb = new WebClient())
                {
                    wb.Headers.Add("user-agent", userAgent);
                    // TODO: check if this works
                    var response = wb.UploadFile(reportUrl, data.ProducedFile);
                }
            }
            else
            {
                // Post equality status
                using (var wb = new WebClient())
                {
                    wb.Headers.Add("user-agent", userAgent);
                    var d = new NameValueCollection();
                    d["equal"] = data.ProducedFile;
                    d["sample"] = data.SampleFile;

                    var response = wb.UploadValues(reportUrl, "POST", d);
                }
            }
        }

        /// <summary>
		/// Gets the success number.
		/// </summary>
		/// <returns>The success number.</returns>
        public int GetSuccessNumber()
        {
            return -1; // Cannot be implemented in this class
        }

        /// <summary>
		/// Saves the report to a given file, with some extra data provided.
		/// </summary>
		/// <param name="pathToFolder">Path to folder to save the report in</param>
		/// <param name="data">The extra result data that should be in the report.</param>
        public string SaveReport(string pathToFolder, ResultData data)
        {
            // Do nothing
            return "";
        }
        #endregion

        /// <summary>
        /// Specific method that sends the runtime to the server for a given sample.
        /// </summary>
        /// <param name="rd">The run data instance which contains the exit code and runtime.</param>
        /// <param name="sample">The name of the sample that was tested.</param>
        public void SendExitCodeAndRuntime(RunData rd, string sample)
        {
            // Post equality status
            using (var wb = new WebClient())
            {
                var d = new NameValueCollection();
                d["exitCode"] = rd.ExitCode+"";
                d["runTime"] = rd.Runtime+"";
                d["sample"] = sample;

                var response = wb.UploadValues(reportUrl, "POST", d);
            }
        }
    }
}
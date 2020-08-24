using System.Collections.Specialized;
using System.Net;
using CCExtractorTester.Analyzers;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;

namespace CCExtractorTester.Comparers
{
    /// <summary>
    /// Class for sending data back to a server instead of doing the comparison locally.
    /// </summary>
    public class ServerComparer : IFileComparable
    {
        private class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }

        private byte[] UploadFiles(string address, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            var request = WebRequest.Create(address);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + "\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, "\r\n"));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes("\r\n");
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// The URL where the class will send status updates to.
        /// </summary>
        private string reportUrl;

        private static string userAgent = "CCX/CI_BOT";

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
            // Check for equality by hash
            if (!data.Dummy && !Hasher.filesAreEqual(data.CorrectFile, data.ProducedFile))
            {
                // Upload result
                using (FileStream stream = File.Open(data.ProducedFile, FileMode.Open))
                {
                    UploadFile[] files = new[] {
                        new UploadFile {
                            Name = "file", //Path.GetFileNameWithoutExtension(data.ProducedFile),
                            Filename = Path.GetFileName(data.ProducedFile),
                            ContentType = "application/octet-stream",
                            Stream = stream
                        }
                    };

                    NameValueCollection nv = new NameValueCollection {
                        { "type", "upload" },
                        { "test_id", data.TestID.ToString() },
                        { "test_file_id", data.TestFileID.ToString() }
                    };

                    byte[] result = UploadFiles(reportUrl, files, nv);
                }

                /*using (var wb = new WebClient())
                {
                    wb.Headers.Add("user-agent", userAgent);
                    // TODO: check if this works
                    var response = wb.UploadFile(reportUrl, data.ProducedFile);
                }*/
            }
            else
            {
                // Post equality status
                using (var wb = new WebClient())
                {
                    wb.Headers.Add("user-agent", userAgent);
                    var d = new NameValueCollection();
                    d["type"] = "equality";
                    d["test_id"] = data.TestID.ToString();
                    d["test_file_id"] = data.TestFileID.ToString();

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
        /// <param name="testId">The id of the test</param>
        public void SendExitCodeAndRuntime(RunData rd, int testId)
        {
            // Post equality status
            using (var wb = new WebClient())
            {
                var d = new NameValueCollection();
                d["exitCode"] = rd.ExitCode.ToString();
                d["runTime"] = Convert.ToInt32(rd.Runtime.TotalMilliseconds).ToString();
                d["test_id"] = testId.ToString();
                d["type"] = "finish";

                var response = wb.UploadValues(reportUrl, "POST", d);
            }
        }
    }
}

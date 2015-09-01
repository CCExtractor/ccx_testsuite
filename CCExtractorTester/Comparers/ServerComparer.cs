using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace CCExtractorTester.Comparers
{
    public class ServerComparer : IFileComparable
    {
        private string reportUrl;

        public ServerComparer(string reportUrl)
        {
            this.reportUrl = reportUrl;
        }

        public void CompareAndAddToResult(CompareData data)
        {
            // Check for equality by hash
            if (!Hasher.filesAreEqual(data.CorrectFile, data.ProducedFile))
            {
                // Upload result
                using (var wb = new WebClient())
                {
                    var response = wb.UploadFile(reportUrl, data.ProducedFile);
                }
                // TODO: finish
            }
            else
            {
                // Post equality status
                using (var wb = new WebClient())
                {
                    var d = new NameValueCollection();
                    d["username"] = "myUser";
                    d["password"] = "myPassword";

                    var response = wb.UploadValues(reportUrl, "POST", d);
                }
                // TODO: finish
            }
        }

        public int GetSuccessNumber()
        {
            return -1; // Cannot be implemented in this class
        }

        public string SaveReport(string pathToFolder, ResultData data)
        {
            // Do nothing
            return "";
        }
    }
}

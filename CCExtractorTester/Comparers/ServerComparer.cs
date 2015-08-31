using System;
using System.Collections.Generic;
using System.Linq;
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

                // TODO: finish
            }
            else
            {
                // Post equality status

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

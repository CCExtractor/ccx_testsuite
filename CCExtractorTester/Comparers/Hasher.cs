using System;
using System.Security.Cryptography;
using System.IO;

namespace CCExtractorTester.Comparers
{
    /// <summary>
    /// This class implements an MD5 hasher to compare files.
    /// </summary>
	public static class Hasher
	{
        /// <summary>
        /// Gets the hash for a given file and given algorithm.
        /// </summary>
        /// <param name="fileLocation">The location of the file to generate a hash for.</param>
        /// <param name="hashAlgorithm">The hash algorithm to use. Defaults to SHA256.</param>
        /// <returns></returns>
        public static string getFileHash(String fileLocation, String hashAlgorithm = "SHA256")
        {
            if (File.Exists(fileLocation))
            {
                using (var stream = File.OpenRead(fileLocation))
                {
                    using (var algo = HashAlgorithm.Create(hashAlgorithm))
                    {
                        return BitConverter.ToString(algo.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Checks if two given files are equal.
        /// </summary>
        /// <param name="file1">The first file.</param>
        /// <param name="file2">The second file.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
		public static bool filesAreEqual(String file1, String file2){
			String first = getFileHash(file1);
			String second = getFileHash(file2);
			return first.CompareTo (second) == 0;				
		}
	}
}
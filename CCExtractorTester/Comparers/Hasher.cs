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
        /// Gets the MD5 hash of a given file.
        /// </summary>
        /// <param name="fileLocation">The location of the file to generate a hash for</param>
        /// <returns>A string wit the MD5 hash, or empty if the file does not exist.</returns>
		public static String getFileMD5Hash(String fileLocation){
			if (File.Exists (fileLocation)) {
				using (var md5 = MD5.Create ()) {
					using (var stream = File.OpenRead (fileLocation)) {
						return BitConverter.ToString (md5.ComputeHash (stream)).Replace ("-", "").ToLower ();
					}
				}
			}
			return "";
		}

        /// <summary>
        /// Checks if two given files are equal.
        /// </summary>
        /// <param name="file1">The first file.</param>
        /// <param name="file2">The second file.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
		public static bool filesAreEqual(String file1, String file2){
			String first = getFileMD5Hash (file1);
			String second = getFileMD5Hash (file2);
			return first.CompareTo (second) == 0;				
		}
	}
}
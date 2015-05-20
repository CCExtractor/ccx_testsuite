using System;
using System.Security.Cryptography;
using System.IO;

namespace CCExtractorTester
{
	public static class Hasher
	{
		public static String getFileMD5Hash(String fileLocation){
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(fileLocation))
				{
					return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","").ToLower();
				}
			}
		}

		public static bool filesAreEqual(String file1, String file2){
			String first = getFileMD5Hash (file1);
			String second = getFileMD5Hash (file2);
			return first.CompareTo (second) == 0;				
		}
	}
}
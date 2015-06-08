using System;

namespace CCExtractorTester
{
	/// <summary>
	/// Interface for comparing files.
	/// </summary>
	public interface IFileComparable
	{
		/// <summary>
		/// Compares the files provided in the data and add to an internal result.
		/// </summary>
		/// <param name="data">The data for this entry.</param>
		void CompareAndAddToResult(CompareData data);
		/// <summary>
		/// Saves the report to a given file, with some extra data provided.
		/// </summary>
		/// <param name="pathToFolder">Path to folder to save the report in</param>
		/// <param name="data">The extra result data that should be in the report.</param>
		/// <returns>The report name</returns>
		String SaveReport (string pathToFolder, ResultData data);
		/// <summary>
		/// Gets the success number.
		/// </summary>
		/// <returns>The success number.</returns>
		int GetSuccessNumber();
	}
}
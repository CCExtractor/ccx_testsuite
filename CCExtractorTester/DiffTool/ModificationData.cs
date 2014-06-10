using System;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// Holds the data of a string and the modifications it has compared to another string.
	/// </summary>
	public class ModificationData
	{
		/// <summary>
		/// Gets or sets an array of hashes from the separate pieces.
		/// </summary>
		public int[] HashedPieces { get; set; }
		/// <summary>
		/// Gets the raw, unsplit data.
		/// </summary>
		/// <value>The raw data.</value>
		public string RawData { get; private set; }
		/// <summary>
		/// Gets or sets an array containing whether each piece was modified or not.
		/// </summary>
		/// <value>The modifications.</value>
		public bool[] Modifications { get; set; }
		/// <summary>
		/// The string split into certain pieces.
		/// </summary>
		/// <value>The pieces.</value>
		public string[] Pieces { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.ModificationData"/> class.
		/// </summary>
		/// <param name="str">String.</param>
		public ModificationData(string str)
		{
			RawData = str;
		}
	}
}
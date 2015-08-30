using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// Represents a single side of a difference view.
	/// </summary>
	public class SingleSideModel
	{
		public List<LineModel> Lines { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.SingleSideModel"/> class.
		/// </summary>
		public SingleSideModel()
		{
			Lines = new List<LineModel>();
		}
	}
}

namespace CCExtractorTester.DiffTool
{
	public enum Edit
	{
		/// <summary>
		/// No edits have been made.
		/// </summary>
		None,
		/// <summary>
		/// Something on the right has been deleted
		/// </summary>
		DeleteRight,
		/// <summary>
		/// Something on the left has been deleted
		/// </summary>
		DeleteLeft,
		/// <summary>
		/// Something was inserted below
		/// </summary>
		InsertDown,
		/// <summary>
		/// Something was inserted above
		/// </summary>
		InsertUp
	}
	/// <summary>
	/// Indicates the start and end of an changed line.
	/// </summary>
	public class EditLengthResult
	{
		/// <summary>
		/// Gets or sets the length of the edit.
		/// </summary>
		/// <value>The length of the edit.</value>
		public int EditLength { get; set; }

		public int StartX { get; set; }
		public int EndX { get; set; }
		public int StartY { get; set; }
		public int EndY { get; set; }

		public Edit LastEdit { get; set; }
	}
}
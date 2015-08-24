
namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// A block of consecutive edits from A and/or B
	/// </summary>
	public class Block
	{
		/// <summary>
		/// Position where deletions in A begin
		/// </summary>
		public int DeleteStartA { get; private set; }

		/// <summary>
		/// The number of deletions in A
		/// </summary>
		public int DeleteCountA { get; private set; }

		/// <summary>
		/// Position where insertion in B begin
		/// </summary>
		public int InsertStartB { get; private set; }

		/// <summary>
		/// The number of insertions in B
		/// </summary>
		public int InsertCountB { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.Block"/> class.
		/// </summary>
		/// <param name="deleteStartA">Delete start a.</param>
		/// <param name="deleteCountA">Delete count a.</param>
		/// <param name="insertStartB">Insert start b.</param>
		/// <param name="insertCountB">Insert count b.</param>
		public Block(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB)
		{
			DeleteStartA = deleteStartA;
			DeleteCountA = deleteCountA;
			InsertStartB = insertStartB;
			InsertCountB = insertCountB;
		}
	}
}

using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	public class Result
	{
		/// <summary>
		/// The chunked pieces of the first text
		/// </summary>
		public string[] PiecesFirst { get; private set; }

		/// <summary>
		/// The chunked pieces of the second text
		/// </summary>
		public string[] PiecesSecond { get; private set; }


		/// <summary>
		/// A collection of DiffBlocks which details deletions and insertions
		/// </summary>
		public IList<Block> DiffBlocks { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.Result"/> class.
		/// </summary>
		/// <param name="piecesFirst">Pieces first.</param>
		/// <param name="piecesSecond">Pieces second.</param>
		/// <param name="blocks">Blocks.</param>
		public Result(string[] piecesFirst, string[] piecesSecond, IList<Block> blocks)
		{
			PiecesFirst = piecesFirst;
			PiecesSecond = piecesSecond;
			DiffBlocks = blocks;
		}
	}
}


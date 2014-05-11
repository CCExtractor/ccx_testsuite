using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	public class Result
	{
		/// <summary>
		/// The chunked peices of the old text
		/// </summary>
		public string[] PiecesOld { get; private set; }

		/// <summary>
		/// The chunked peices of the new text
		/// </summary>
		public string[] PiecesNew { get; private set; }


		/// <summary>
		/// A collection of DiffBlocks which details deletions and insertions
		/// </summary>
		public IList<Block> DiffBlocks { get; private set; }

		public Result(string[] peicesOld, string[] piecesNew, IList<Block> blocks)
		{
			PiecesOld = peicesOld;
			PiecesNew = piecesNew;
			DiffBlocks = blocks;
		}
	}
}


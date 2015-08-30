using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// Side by side builder.
	/// </summary>
	public class SideBySideBuilder
	{
		private readonly DifferTool differ;

		delegate void PieceBuilder(string firstText, string secondText, List<LineModel> firstPieces, List<LineModel> secondPieces);

		public static readonly char[] WordSeparators = new[] {' ', '\t', '.', '(', ')', '{', '}', ','};

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.SideBySideBuilder"/> class.
		/// </summary>
		/// <param name="differ">Differ.</param>
		public SideBySideBuilder(DifferTool differ)
		{
			if (differ == null) throw new ArgumentNullException("differ");

			this.differ = differ;
		}

		/// <summary>
		/// Builds the diff model.
		/// </summary>
		/// <returns>The diff model.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		public SideBySideModel BuildDiffModel(string firstText, string secondText)
		{
			if (firstText == null) throw new ArgumentNullException("first given text is null");
			if (secondText == null) throw new ArgumentNullException("second given text is null");

			return BuildLineDiff(firstText, secondText);
		}

		/// <summary>
		/// Builds the line diff.
		/// </summary>
		/// <returns>The line diff.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		private SideBySideModel BuildLineDiff(string firstText, string secondText)
		{
			var model = new SideBySideModel();
			var diffResult = differ.CreateLineDifferences(firstText, secondText, true);
			BuildDiffPieces(diffResult, model.FirstText.Lines, model.SecondText.Lines, BuildWordDiffPieces);
			return model;
		}

		/// <summary>
		/// Builds the word diff pieces.
		/// </summary>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="firstPieces">First pieces.</param>
		/// <param name="secondPieces">Second pieces.</param>
		private void BuildWordDiffPieces(string firstText, string secondText, List<LineModel> firstPieces, List<LineModel> secondPieces)
		{
			var diffResult = differ.CreateWordDifferences(firstText, secondText, false, WordSeparators);
			BuildDiffPieces(diffResult, firstPieces, secondPieces, null);
		}

		/// <summary>
		/// Builds the diff pieces.
		/// </summary>
		/// <param name="diffResult">Diff result.</param>
		/// <param name="firstPieces">First pieces.</param>
		/// <param name="secondPieces">Second pieces.</param>
		/// <param name="subPieceBuilder">Sub piece builder.</param>
		private static void BuildDiffPieces(Result diffResult, List<LineModel> firstPieces, List<LineModel> secondPieces, PieceBuilder subPieceBuilder)
		{
			int aPos = 0;
			int bPos = 0;

			foreach (var diffBlock in diffResult.DiffBlocks)
			{
				while (bPos < diffBlock.InsertStartB && aPos < diffBlock.DeleteStartA)
				{
					firstPieces.Add(new LineModel(diffResult.PiecesFirst[aPos], ChangeType.Unchanged, aPos + 1));
					secondPieces.Add(new LineModel(diffResult.PiecesSecond[bPos], ChangeType.Unchanged, bPos + 1));
					aPos++;
					bPos++;
				}

				int i = 0;
				for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
				{
					var oldPiece = new LineModel(diffResult.PiecesFirst[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1);
					var newPiece = new LineModel(diffResult.PiecesSecond[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1);

					if (subPieceBuilder != null)
					{
						subPieceBuilder(diffResult.PiecesFirst[aPos], diffResult.PiecesSecond[bPos], oldPiece.SubPieces, newPiece.SubPieces);
						newPiece.Type = oldPiece.Type = ChangeType.Modified;
					}

					firstPieces.Add(oldPiece);
					secondPieces.Add(newPiece);
					aPos++;
					bPos++;
				}

				if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
				{
					for (; i < diffBlock.DeleteCountA; i++)
					{
						firstPieces.Add(new LineModel(diffResult.PiecesFirst[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1));
						secondPieces.Add(new LineModel());
						aPos++;
					}
				}
				else
				{
					for (; i < diffBlock.InsertCountB; i++)
					{
						secondPieces.Add(new LineModel(diffResult.PiecesSecond[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
						firstPieces.Add(new LineModel());
						bPos++;
					}
				}
			}

			while (bPos < diffResult.PiecesSecond.Length && aPos < diffResult.PiecesFirst.Length)
			{
				firstPieces.Add(new LineModel(diffResult.PiecesFirst[aPos], ChangeType.Unchanged, aPos + 1));
				secondPieces.Add(new LineModel(diffResult.PiecesSecond[bPos], ChangeType.Unchanged, bPos + 1));
				aPos++;
				bPos++;
			}
		}		
	}
}
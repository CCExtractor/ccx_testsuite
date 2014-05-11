using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	public class SideBySideBuilder
	{
		private readonly DifferTool differ;

		delegate void PieceBuilder(string oldText, string newText, List<LineModel> oldPieces, List<LineModel> newPieces);

		public static readonly char[] WordSeparaters = new[] {' ', '\t', '.', '(', ')', '{', '}', ','};

		public SideBySideBuilder(DifferTool differ)
		{
			if (differ == null) throw new ArgumentNullException("differ");

			this.differ = differ;
		}

		public SideBySideModel BuildDiffModel(string oldText, string newText)
		{
			if (oldText == null) throw new ArgumentNullException("oldText");
			if (newText == null) throw new ArgumentNullException("newText");

			return BuildLineDiff(oldText, newText);
		}

		private SideBySideModel BuildLineDiff(string oldText, string newText)
		{
			var model = new SideBySideModel();
			var diffResult = differ.CreateLineDiffs(oldText, newText, true);
			BuildDiffPieces(diffResult, model.OldText.Lines, model.NewText.Lines, BuildWordDiffPieces);
			return model;
		}

		private void BuildWordDiffPieces(string oldText, string newText, List<LineModel> oldPieces, List<LineModel> newPieces)
		{
			var diffResult = differ.CreateWordDiffs(oldText, newText, false, WordSeparaters);
			BuildDiffPieces(diffResult, oldPieces, newPieces, null);
		}

		private static void BuildDiffPieces(Result diffResult, List<LineModel> oldPieces, List<LineModel> newPieces, PieceBuilder subPieceBuilder)
		{
			int aPos = 0;
			int bPos = 0;

			foreach (var diffBlock in diffResult.DiffBlocks)
			{
				while (bPos < diffBlock.InsertStartB && aPos < diffBlock.DeleteStartA)
				{
					oldPieces.Add(new LineModel(diffResult.PiecesOld[aPos], ChangeType.Unchanged, aPos + 1));
					newPieces.Add(new LineModel(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
					aPos++;
					bPos++;
				}

				int i = 0;
				for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
				{
					var oldPiece = new LineModel(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1);
					var newPiece = new LineModel(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1);

					if (subPieceBuilder != null)
					{
						subPieceBuilder(diffResult.PiecesOld[aPos], diffResult.PiecesNew[bPos], oldPiece.SubPieces, newPiece.SubPieces);
						newPiece.Type = oldPiece.Type = ChangeType.Modified;
					}

					oldPieces.Add(oldPiece);
					newPieces.Add(newPiece);
					aPos++;
					bPos++;
				}

				if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
				{
					for (; i < diffBlock.DeleteCountA; i++)
					{
						oldPieces.Add(new LineModel(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1));
						newPieces.Add(new LineModel());
						aPos++;
					}
				}
				else
				{
					for (; i < diffBlock.InsertCountB; i++)
					{
						newPieces.Add(new LineModel(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
						oldPieces.Add(new LineModel());
						bPos++;
					}
				}
			}

			while (bPos < diffResult.PiecesNew.Length && aPos < diffResult.PiecesOld.Length)
			{
				oldPieces.Add(new LineModel(diffResult.PiecesOld[aPos], ChangeType.Unchanged, aPos + 1));
				newPieces.Add(new LineModel(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
				aPos++;
				bPos++;
			}
		}		
	}
}


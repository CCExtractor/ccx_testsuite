using System;
using System.Collections.Generic;
using System.Linq;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// A class used to create differences from two files (or strings)
	/// </summary>
	public class DifferTool
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.DifferTool"/> class.
		/// </summary>
		public DifferTool (){}

		/// <summary>
		/// Creates a result containing the differences between the two given strings.
		/// </summary>
		/// <returns>A model holding all the differences</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		public Result CreateLineDifferences(string firstText, string secondText, bool ignoreWhitespace)
		{
			return CreateLineDifferences(firstText, secondText, ignoreWhitespace, false);
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings.
		/// </summary>
		/// <returns>A model holding all the differences</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		public Result CreateLineDifferences(string firstText, string secondText, bool ignoreWhitespace, bool ignoreCase)
		{
			if (firstText == null) throw new ArgumentNullException("first text provided is null");
			if (secondText == null) throw new ArgumentNullException("second text provided is null");

			return CreateCustomDifferences(firstText, secondText, ignoreWhitespace,ignoreCase, str => NormalizeNewlines(str).Split('\n'));
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, character by character.
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		public Result CreateCharacterDifferences(string firstText, string secondText, bool ignoreWhitespace)
		{
			return CreateCharacterDifferences(firstText, secondText, ignoreWhitespace, false);
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, character by character.
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		public Result CreateCharacterDifferences(string firstText, string secondText, bool ignoreWhitespace, bool ignoreCase)
		{
			if (firstText == null) throw new ArgumentNullException("first text provided is null");
			if (secondText == null) throw new ArgumentNullException("second text provided is null");

			return CreateCustomDifferences(
				firstText,
				secondText,
				ignoreWhitespace,
				ignoreCase,
				str =>
				{
					var s = new string[str.Length];
					for (int i = 0; i < str.Length; i++) s[i] = str[i].ToString();
					return s;
				});
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, word by word. Gets split on the given separators
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		/// <param name="separators">Separators.</param>
		public Result CreateWordDifferences(string firstText, string secondText, bool ignoreWhitespace, char[] separators)
		{
			return CreateWordDifferences(firstText, secondText, ignoreWhitespace, false, separators);
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, word by word. Gets split on the given separators
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhitespace">If set to <c>true</c> ignore whitespace.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		/// <param name="separators">Separators.</param>
		public Result CreateWordDifferences(string firstText, string secondText, bool ignoreWhitespace, bool ignoreCase, char[] separators)
		{
			if (firstText == null) throw new ArgumentNullException("first given text is null");
			if (secondText == null) throw new ArgumentNullException("second given text is null");

			return CreateCustomDifferences(
				firstText,
				secondText,
				ignoreWhitespace,
				ignoreCase,
				str => SmartSplit(str, separators));
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, using a given predicate to split the strings.
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhiteSpace">If set to <c>true</c> ignore white space.</param>
		/// <param name="stringSplitter">The predicate used to split the strings in pieces to compare.</param>
		public Result CreateCustomDifferences(string firstText, string secondText, bool ignoreWhiteSpace, Func<string, string[]> stringSplitter)
		{
			return CreateCustomDifferences(firstText, secondText, ignoreWhiteSpace, false, stringSplitter);
		}
		/// <summary>
		/// Creates a result containing the differences between the two given strings, using a given predicate to split the strings.
		/// </summary>
		/// <returns>A model holding all the differences.</returns>
		/// <param name="firstText">First text.</param>
		/// <param name="secondText">Second text.</param>
		/// <param name="ignoreWhiteSpace">If set to <c>true</c> ignore white space.</param>
		/// <param name="ignoreCase">If set to <c>true</c> ignore case.</param>
		/// <param name="stringSplitter">Predicate used to split the strings in pieces for comparison.</param>
		public Result CreateCustomDifferences(string firstText, string secondText, bool ignoreWhiteSpace, bool ignoreCase, Func<string, string[]> stringSplitter)
		{
			if (firstText == null) throw new ArgumentNullException("Given first text is null");
			if (secondText == null) throw new ArgumentNullException("Given second text is null");
			if (stringSplitter == null) throw new ArgumentNullException("The provided splitter predicate is null");

			var pieceHash = new Dictionary<string, int>();
			var lineDifferences = new List<Block>();

			var modificationFirst = new ModificationData(firstText);
			var modificationSecond = new ModificationData(secondText);

			CreatePieceHashes(pieceHash, modificationFirst, ignoreWhiteSpace, ignoreCase, stringSplitter);
			CreatePieceHashes(pieceHash, modificationSecond, ignoreWhiteSpace, ignoreCase, stringSplitter);

			BuildModificationData(modificationFirst, modificationSecond);

			int lengthFirstPieces = modificationFirst.HashedPieces.Length;
			int lengthSecondPieces = modificationSecond.HashedPieces.Length;
			int positionFirst = 0,positionSecond = 0;

			do
			{
				while (positionFirst < lengthFirstPieces
					&& positionSecond < lengthSecondPieces
					&& !modificationFirst.Modifications[positionFirst]
					&& !modificationSecond.Modifications[positionSecond])
				{
					positionFirst++;
					positionSecond++;
				}

				int startOfFirst = positionFirst;
				int startOfSecond = positionSecond;
				for (; positionFirst < lengthFirstPieces && modificationFirst.Modifications[positionFirst]; positionFirst++) ;

				for (; positionSecond < lengthSecondPieces && modificationSecond.Modifications[positionSecond]; positionSecond++) ;

				int numberRemoved = positionFirst - startOfFirst;
				int numberInsertd = positionSecond - startOfSecond;
				if (numberRemoved > 0 || numberInsertd > 0)
				{
					lineDifferences.Add(new Block(startOfFirst, numberRemoved, startOfSecond, numberInsertd));
				}
			} while (positionFirst < lengthFirstPieces && positionSecond < lengthSecondPieces);

			return new Result(modificationFirst.Pieces, modificationSecond.Pieces, lineDifferences);
		}
		/// <summary>
		/// Normalizes the newlines by replacing \r\n with \n and \r with \n.
		/// </summary>
		/// <returns>The newlines.</returns>
		/// <param name="str">String.</param>
		private static string NormalizeNewlines(string str)
		{
			return str.Replace("\r\n", "\n").Replace("\r", "\n");
		}
		/// <summary>
		/// Splits a given string in a smart way.
		/// </summary>
		/// <returns>The chunks</returns>
		/// <param name="str">The string to split</param>
		/// <param name="delimiters">The delimiters that are used to split the string..</param>
		private static string[] SmartSplit(string str, IEnumerable<char> delimiters)
		{
			var list = new List<string>();
			int begin = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (delimiters.Contains(str[i]))
				{
					list.Add(str.Substring(begin, (i + 1 - begin)));
					begin = i + 1;
				}
				else if (i >= str.Length - 1)
				{
					list.Add(str.Substring(begin, (i + 1 - begin)));
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Finds the middle snake and the minimum length of the edit script comparing string A and B
		/// </summary>
		/// <param name="A"></param>
		/// <param name="startA">Lower bound inclusive</param>
		/// <param name="endA">Upper bound exclusive</param>
		/// <param name="B"></param>
		/// <param name="startB">lower bound inclusive</param>
		/// <param name="endB">upper bound exclusive</param>
		/// <returns></returns>
		protected static EditLengthResult CalculateEditLength(int[] A, int startA, int endA, int[] B, int startB, int endB)
		{
			int N = endA - startA;
			int M = endB - startB;
			int MAX = M + N + 1;

			var forwardDiagonal = new int[MAX + 1];
			var reverseDiagonal = new int[MAX + 1];
			return CalculateEditLength(A, startA, endA, B, startB, endB, forwardDiagonal, reverseDiagonal);
		}

		private static EditLengthResult CalculateEditLength(int[] A, int startA, int endA, int[] B, int startB, int endB, int[] forwardDiagonal, int[] reverseDiagonal)
		{
			if (null == A) throw new ArgumentNullException("A");
			if (null == B) throw new ArgumentNullException("B");

			if (A.Length == 0 && B.Length == 0)
			{
				return new EditLengthResult();
			}

			Edit lastEdit;
			int N = endA - startA;
			int M = endB - startB;
			int MAX = M + N + 1;
			int HALF = MAX / 2;
			int delta = N - M;
			bool deltaEven = delta % 2 == 0;
			forwardDiagonal[1 + HALF] = 0;
			reverseDiagonal[1 + HALF] = N + 1;

			for (int D = 0; D <= HALF; D++)
			{
				// forward D-path
				for (int k = -D; k <= D; k += 2)
				{
					int kIndex = k + HALF;
					int x, y;
					if (k == -D || (k != D && forwardDiagonal[kIndex - 1] < forwardDiagonal[kIndex + 1]))
					{
						x = forwardDiagonal[kIndex + 1]; // y up    move down from previous diagonal
						lastEdit = Edit.InsertDown;
					}
					else
					{
						x = forwardDiagonal[kIndex - 1] + 1; // x up     move right from previous diagonal
						lastEdit = Edit.DeleteRight;
					}
					y = x - k;
					int startX = x;
					int startY = y;
					while (x < N && y < M && A[x + startA] == B[y + startB])
					{
						x += 1;
						y += 1;
					}

					forwardDiagonal[kIndex] = x;

					if (!deltaEven)
					{
						int revX, revY;
						if (k - delta >= (-D + 1) && k - delta <= (D - 1))
						{
							int revKIndex = (k - delta) + HALF;
							revX = reverseDiagonal[revKIndex];
							revY = revX - k;
							if (revX <= x && revY <= y)
							{
								var res = new EditLengthResult();
								res.EditLength = 2 * D - 1;
								res.StartX = startX + startA;
								res.StartY = startY + startB;
								res.EndX = x + startA;
								res.EndY = y + startB;
								res.LastEdit = lastEdit;
								return res;
							}
						}
					}
				}

				// reverse D-path
				for (int k = -D; k <= D; k += 2)
				{
					int kIndex = k + HALF;
					int x, y;
					if (k == -D || (k != D && reverseDiagonal[kIndex + 1] <= reverseDiagonal[kIndex - 1]))
					{
						x = reverseDiagonal[kIndex + 1] - 1; // move left from k+1 diagonal
						lastEdit = Edit.DeleteLeft;
					}
					else
					{
						x = reverseDiagonal[kIndex - 1]; //move up from k-1 diagonal
						lastEdit = Edit.InsertUp;
					}
					y = x - (k + delta);

					int endX = x;
					int endY = y;

					while (x > 0 && y > 0 && A[startA + x - 1] == B[startB + y - 1])
					{
						x -= 1;
						y -= 1;
					}

					reverseDiagonal[kIndex] = x;

					if (deltaEven)
					{
						int forX, forY;
						if (k + delta >= -D && k + delta <= D)
						{
							int forKIndex = (k + delta) + HALF;
							forX = forwardDiagonal[forKIndex];
							forY = forX - (k + delta);
							if (forX >= x && forY >= y)
							{
								var res = new EditLengthResult();
								res.EditLength = 2 * D;
								res.StartX = x + startA;
								res.StartY = y + startB;
								res.EndX = endX + startA;
								res.EndY = endY + startB;
								res.LastEdit = lastEdit;
								return res;
							}
						}
					}
				}
			}
			throw new Exception("Should never get here");
		}

		protected static void BuildModificationData(ModificationData A, ModificationData B)
		{
			int N = A.HashedPieces.Length;
			int M = B.HashedPieces.Length;
			int MAX = M + N + 1;
			var forwardDiagonal = new int[MAX + 1];
			var reverseDiagonal = new int[MAX + 1];
			BuildModificationData(A, 0, N, B, 0, M, forwardDiagonal, reverseDiagonal);
		}

		private static void BuildModificationData(ModificationData A,int startA,int endA,ModificationData B,int startB,int endB,int[] forwardDiagonal,int[] reverseDiagonal)
		{

			while (startA < endA && startB < endB && A.HashedPieces[startA].Equals(B.HashedPieces[startB]))
			{
				startA++;
				startB++;
			}
			while (startA < endA && startB < endB && A.HashedPieces[endA - 1].Equals(B.HashedPieces[endB - 1]))
			{
				endA--;
				endB--;
			}

			int aLength = endA - startA;
			int bLength = endB - startB;
			if (aLength > 0 && bLength > 0)
			{
				EditLengthResult res = CalculateEditLength(A.HashedPieces, startA, endA, B.HashedPieces, startB, endB, forwardDiagonal, reverseDiagonal);
				if (res.EditLength <= 0) return;

				if (res.LastEdit == Edit.DeleteRight && res.StartX - 1 > startA)
					A.Modifications[--res.StartX] = true;
				else if (res.LastEdit == Edit.InsertDown && res.StartY - 1 > startB)
					B.Modifications[--res.StartY] = true;
				else if (res.LastEdit == Edit.DeleteLeft && res.EndX < endA)
					A.Modifications[res.EndX++] = true;
				else if (res.LastEdit == Edit.InsertUp && res.EndY < endB)
					B.Modifications[res.EndY++] = true;

				BuildModificationData(A, startA, res.StartX, B, startB, res.StartY, forwardDiagonal, reverseDiagonal);

				BuildModificationData(A, res.EndX, endA, B, res.EndY, endB, forwardDiagonal, reverseDiagonal);
			}
			else if (aLength > 0)
			{
				for (int i = startA; i < endA; i++)
					A.Modifications[i] = true;
			}
			else if (bLength > 0)
			{
				for (int i = startB; i < endB; i++)
					B.Modifications[i] = true;
			}
		}

		private static void CreatePieceHashes(IDictionary<string, int> pieceHash, ModificationData data, bool ignoreWhitespace, bool ignoreCase, Func<string, string[]> chunker)
		{
			string[] pieces;

			if (string.IsNullOrEmpty(data.RawData))
				pieces = new string[0];
			else
				pieces = chunker(data.RawData);

			data.Pieces = pieces;
			data.HashedPieces = new int[pieces.Length];
			data.Modifications = new bool[pieces.Length];

			for (int i = 0; i < pieces.Length; i++)
			{
				string piece = pieces[i];
				if (ignoreWhitespace) piece = piece.Trim();
				if (ignoreCase) piece = piece.ToUpperInvariant();

				if (pieceHash.ContainsKey(piece))
				{
					data.HashedPieces[i] = pieceHash[piece];
				}
				else
				{
					data.HashedPieces[i] = pieceHash.Count;
					pieceHash[piece] = pieceHash.Count;
				}
			}
		}
	}
}
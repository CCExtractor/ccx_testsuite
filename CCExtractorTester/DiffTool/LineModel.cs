using System;
using System.Collections.Generic;

namespace CCExtractorTester.DiffTool
{
	public enum ChangeType
	{
		Unchanged,
		Deleted,
		Inserted,
		Imaginary,
		Modified
	}

	/// <summary>
	/// Line model. Represents a single line of the differences model.
	/// </summary>
	public class LineModel
	{
		public ChangeType Type { get; set; }
		public int? Position { get; set; }
		public string Text { get; set; }
		public List<LineModel> SubPieces { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.LineModel"/> class.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="type">Type.</param>
		/// <param name="position">Position.</param>
		public LineModel(string text, ChangeType type, int? position)
		{
			Text = text;
			Position = position;
			Type = type;
			SubPieces = new List<LineModel>();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.LineModel"/> class.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="type">Type.</param>
		public LineModel(string text, ChangeType type): this(text, type, null){}
		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.LineModel"/> class.
		/// </summary>
		public LineModel(): this(null, ChangeType.Imaginary){}
	}
}
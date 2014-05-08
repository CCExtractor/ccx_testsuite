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

	public class LineModel
	{
		public ChangeType Type { get; set; }
		public int? Position { get; set; }
		public string Text { get; set; }
		public List<LineModel> SubPieces { get; set; }

		public LineModel(string text, ChangeType type, int? position)
		{
			Text = text;
			Position = position;
			Type = type;
			SubPieces = new List<LineModel>();
		}

		public LineModel(string text, ChangeType type)
			: this(text, type, null)
		{
		}

		public LineModel()
			: this(null, ChangeType.Imaginary)
		{
		}
	}
}

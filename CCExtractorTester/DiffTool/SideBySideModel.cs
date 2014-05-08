using System;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// A model which represents differences between to texts to be shown side by side
	/// </summary>
	public class SideBySideModel
	{
		public SingleSideModel OldText { get; private set; }
		public SingleSideModel NewText { get; private set; }

		public SideBySideModel()
		{
			OldText = new SingleSideModel();
			NewText = new SingleSideModel();
		}

		public string CreateHTML(){
			string html = @"
<html>
	<head>
		<title>Difference between two files</title>
		<style type=""text/css"">{0}</style>
		<script type=""text/javascript"">{1}</script>
		<script type=""text/javascript"">{2}</script>
	</head>
	<body>
	</body>
</html>";
			return String.Format (html, DiffResources.Diff1, DiffResources.jquery_1_11_1_min, DiffResources.Diff);
		}
	}
}


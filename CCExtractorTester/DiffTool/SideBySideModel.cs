using System;
using System.Text;
using System.Web;
using System.Globalization;

namespace CCExtractorTester.DiffTool
{
	/// <summary>
	/// A model which represents differences between to texts to be shown side by side
	/// </summary>
	public class SideBySideModel
	{
		public SingleSideModel FirstText { get; private set; }
		public SingleSideModel SecondText { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffTool.SideBySideModel"/> class.
		/// </summary>
		public SideBySideModel()
		{
			FirstText = new SingleSideModel();
			SecondText = new SingleSideModel();
		}

		/// <summary>
		/// Gets the number of changes.
		/// </summary>
		/// <returns>The number of changes.</returns>
		public int GetNumberOfChanges ()
		{
			return FirstText.Lines.Count - FirstText.Lines.FindAll (l => l.Type == ChangeType.Unchanged).Count;
		}

		/// <summary>
		/// Creates the HTML.
		/// </summary>
		/// <returns>The HTML.</returns>
		public string CreateHTML(){
			return GetHTML(GetDiffHTML());
		}

		/// <summary>
		/// Gets the diff HTML.
		/// </summary>
		/// <returns>The diff HTML.</returns>
		/// <param name="extraAttributesDiffBox">Extra attributes for the diffBox html div</param>
		/// <param name="reduce">If set to <c>true</c>, reduce to only show the changed lines.</param>
		public string GetDiffHTML(string extraAttributesDiffBox = "",bool reduce=false){
			string model = @"				
				<div class=""diffBox"" {0}>
			        <div class=""leftPane"">
			             <div class=""diffHeader"">Correct sample file</div>
			             {1}
			        </div>
			        <div class=""rightPane"">
			           <div class=""diffHeader"">Generated output file</div>
			           {2}
			        </div>
			        <div class=""clear"">
			        </div>
			    </div>";
			return String.Format (model,extraAttributesDiffBox,GetSideDiffHTML (FirstText,reduce), GetSideDiffHTML (SecondText,reduce));
		}

		/// <summary>
		/// Gets the HTML for a single side.
		/// </summary>
		/// <returns>The HTML for a single side.</returns>
		/// <param name="side">A single side of the diff model.</param>
		/// <param name="reduce">If set to <c>true</c>, reduce to only show the changed lines.</param>
		private string GetSideDiffHTML(SingleSideModel side,bool reduce=false){
			string model = @"
				<div class=""diffPane"">
				    <table cellpadding=""0"" cellspacing=""0"" class=""diffTable"">
				        {0}
				    </table>
				</div>";
			StringBuilder sb = new StringBuilder ();
			foreach (LineModel lm in side.Lines) {
				if (reduce && lm.Type.Equals (ChangeType.Unchanged)) {
					continue;
				}
				sb.AppendFormat (@"
		<tr>
            <td class=""lineNumber"">{0}</td>
            <td class=""line {1}Line"">
                <span class=""lineText"">
                    {2}
				</span>
            </td>
        </tr>", lm.Position.HasValue ? lm.Position.ToString () : "&nbsp;",lm.Type.ToString(),GetLineHTML(lm));
			}
			return String.Format (model, sb.ToString ());
		}
		/// <summary>
		/// Gets the line HTML for a single line.
		/// </summary>
		/// <returns>The line HTML.</returns>
		/// <param name="lm">The line model.</param>
		private string GetLineHTML(LineModel lm){
			if (!string.IsNullOrEmpty(lm.Text))
			{
				string spaceValue = "&#183;";
				string tabValue = "&#183;&#183;";
				if (lm.Type == ChangeType.Deleted || lm.Type == ChangeType.Inserted || lm.Type == ChangeType.Unchanged)
				{
					return HttpUtility.HtmlEncode(lm.Text).Replace (" ", spaceValue).Replace ("\t", tabValue);
				}
				else if (lm.Type == ChangeType.Modified)
				{
					StringBuilder sb = new StringBuilder ();
					foreach (LineModel character in lm.SubPieces)
					{
						if (character.Type == ChangeType.Imaginary) continue;
						sb.AppendFormat (@"<span class=""{0}Character"">{1}</span>", character.Type.ToString (), HttpUtility.HtmlEncode (character.Text).Replace (" ", spaceValue.ToString ()));
					}
					return sb.ToString ();
				}

			}
			return "";
		}
		/// <summary>
		/// Gets the HTML.
		/// </summary>
		/// <returns>The HTML.</returns>
		/// <param name="body">The body of this page.</param>
		/// <param name="title">Title.</param>
		/// <param name="additionalHeader">Additional header.</param>
		public static string GetHTML(string body,string title="Comparing 2 strings",string additionalHeader=""){
			string html = @"
				<html>
					<head>
						<title>{0}</title>
						<style type=""text/css"">{1}</style>
						{2}
					</head>
					<body>
				 	{3}
					</body>
				</html>";
			return String.Format (html, title,GetCSS(),additionalHeader,body);
		}
		/// <summary>
		/// Gets the CSS.
		/// </summary>
		/// <returns>The CSS string.</returns>
		public static string GetCSS(){
			DiffResources.Culture = CultureInfo.InvariantCulture;
			return DiffResources.Diff1;
		}
	}
}
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
		public SingleSideModel OldText { get; private set; }
		public SingleSideModel NewText { get; private set; }

		public SideBySideModel()
		{
			OldText = new SingleSideModel();
			NewText = new SingleSideModel();
		}

		public int GetNumberOfChanges ()
		{
			return OldText.Lines.Count - OldText.Lines.FindAll (l => l.Type == ChangeType.Unchanged).Count;
		}

		public string CreateHTML(){
			return GetHTML(GetDiffHTML());
		}

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
			return String.Format (model,extraAttributesDiffBox,GetSideDiffHTML (OldText,reduce), GetSideDiffHTML (NewText,reduce));
		}

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
			DiffResources.Culture = CultureInfo.InvariantCulture;
			return String.Format (html, title,DiffResources.Diff1,additionalHeader,body);
		}


	}
}


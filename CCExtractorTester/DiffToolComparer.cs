using System;
using System.Text;
using CCExtractorTester.DiffTool;
using System.IO;

namespace CCExtractorTester
{
	public class DiffToolComparer : IFileComparable
	{
		private StringBuilder Builder { get; set; }
		private SideBySideBuilder Differ { get; set; }

		public DiffToolComparer ()
		{
			Builder = new StringBuilder ();
			Differ = new SideBySideBuilder (new DifferTool ());
		}

		#region IFileComparable implementation

		public string GetReportFileName ()
		{
			return "Report_" + DateTime.Now.ToFileTime () + ".html";
		}

		public void CompareAndAddToResult (string fileLocation1, string fileLocation2,string extraHTML="")
		{
			string oldText = string.Empty;
			using (StreamReader streamReader = new StreamReader(fileLocation1, Encoding.UTF8))
			{            
				oldText = streamReader.ReadToEnd();
			}
			string newText = string.Empty;
			using (StreamReader streamReader = new StreamReader(fileLocation2, Encoding.UTF8))
			{            
				newText = streamReader.ReadToEnd();
			}

			SideBySideModel sbsm = Differ.BuildDiffModel (oldText, newText);
			int changes = sbsm.GetNumberOfChanges ();
			String extra = "";
			if (!String.IsNullOrEmpty (extraHTML)) {
				extra = "<br>" + extraHTML;
			}
			Builder.AppendFormat (@"<p>Files compared: {0} vs {1} {2}<br />Changes detected: {3} (<span onclick=""toggleNext(this);"">click here to expand<span>)</p>",fileLocation1,fileLocation2,extra,changes);
			Builder.Append (sbsm.GetDiffHTML (@"style=""display:none;"""));
		}

		public string GetResult ()
		{
			string javascript = @"
				<script type=""text/javascript"">
					function toggleNext(elm){
						var next = elm.parentNode.nextElementSibling;
						if(next.style.display == ""none""){
							next.style.display = ""block"";
						} else {
							next.style.display = ""none"";
						}
					}
				</script>";
			return SideBySideModel.GetHTML(Builder.ToString (),"Report "+DateTime.Now.ToShortDateString(),javascript);
		}
		#endregion
	}
}
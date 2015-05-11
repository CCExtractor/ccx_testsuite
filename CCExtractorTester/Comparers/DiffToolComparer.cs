using System;
using System.Text;
using CCExtractorTester.DiffTool;
using System.IO;
using System.Linq;

namespace CCExtractorTester
{
	/// <summary>
	/// Difference comparer using the built-in tool. Generates HTML.
	/// </summary>
	public class DiffToolComparer : IFileComparable
	{
		/// <summary>
		/// Gets or sets the name of the temp file.
		/// </summary>
		/// <value>The name of the temp file.</value>
		private string TempFileName { get; set; }
		/// <summary>
		/// Gets or sets the stringbuilder.
		/// </summary>
		/// <value>The builder.</value>
		private StringBuilder Builder { get; set; }
		/// <summary>
		/// Gets or sets the builder diff. Uses streamwriter for preventing out-of-memory exceptions.
		/// </summary>
		/// <value>The builder diff.</value>
		private StreamWriter BuilderDiff { get; set; }
		/// <summary>
		/// Gets or sets the instance that actually does the differ.
		/// </summary>
		/// <value>The differ.</value>
		private SideBySideBuilder Differ { get; set; }
		/// <summary>
		/// Gets or sets the number of entries that have been processed so far.
		/// </summary>
		/// <value>The count.</value>
		private int Count { get; set; }
		/// <summary>
		/// If <c>true</c>, only the differences will be saved to the report. Otherwise the entire file will be shown in the differ.
		/// </summary>
		/// <value><c>true</c> if reduce; otherwise, <c>false</c>.</value>
		private bool Reduce { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CCExtractorTester.DiffToolComparer"/> class.
		/// </summary>
		/// <param name="reduce">If set to <c>true</c>, only show the changed lines.</param>
		public DiffToolComparer (bool reduce=false)
		{
			Builder = new StringBuilder ();
			TempFileName = "tmpHTML" + (DateTime.UtcNow - new DateTime (1970, 1, 1)).TotalSeconds + ".html";
			BuilderDiff = new StreamWriter(TempFileName, false);
			Differ = new SideBySideBuilder (new DifferTool ());
			Count = 0;
			Reduce = reduce;
		}

		#region IFileComparable implementation
		/// <summary>
		/// Gets the name of the report file.
		/// </summary>
		/// <returns>The report file name.</returns>
		public string GetReportFileName ()
		{
			return "Report_" + DateTime.Now.ToFileTime () + ".html";
		}
		/// <summary>
		/// Compares the files provided in the data and add to an internal result.
		/// </summary>
		/// <param name="data">The data for this entry.</param>
		public void CompareAndAddToResult (CompareData data)
		{
			string oldText = string.Empty;
			using (StreamReader streamReader = new StreamReader(data.CorrectFile, Encoding.UTF8))
			{            
				oldText = streamReader.ReadToEnd();
			}
			string newText = string.Empty;
			using (StreamReader streamReader = new StreamReader(data.ProducedFile, Encoding.UTF8))
			{            
				newText = streamReader.ReadToEnd();
			}

			SideBySideModel sbsm = Differ.BuildDiffModel (oldText, newText);
			int changes = sbsm.GetNumberOfChanges ();
			string onclick = "";
			string clss = "green";
			if (changes > 0) {
				lock (this) {
					BuilderDiff.WriteLine (sbsm.GetDiffHTML (String.Format (@"style=""display:none;"" id=""{0}""", "entry_" + Count), Reduce));
					BuilderDiff.Flush ();
				}
				onclick = String.Format (@"onclick=""toggle('{0}'); mark(this);""", "entry_" + Count);
				clss = "red";				
			}
			Builder.AppendFormat (
				@"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td class=""{3}"" {4}>{5}</td></tr>",
				data.SampleFile,
				data.Command,
				data.RunTime.ToString(),
				clss,
				onclick,
				changes);
			Count++;
		}
		/// <summary>
		/// Saves the report to a given file, with some extra data provided.
		/// </summary>
		/// <param name="pathToFolder">Path to folder to save the report in</param>
		/// <param name="data">The extra result data that should be in the report.</param>
		public void SaveReport (string pathToFolder, ResultData data)
		{
			string additionalHeader = @"
				<script type=""text/javascript"">
					function toggleNext(elm){
						var next = elm.parentNode.nextElementSibling;
						if(next.style.display == ""none""){
							next.style.display = ""block"";
						} else {
							next.style.display = ""none"";
						}
					}
					function toggle(id){
						var next = document.getElementById(id);
						if(next.style.display == ""none""){
							next.style.display = ""block"";
						} else {
							next.style.display = ""none"";
						}
					}
					function mark(elm){
						var clsses = elm.className;
						if(clsses.indexOf(""mark"") > -1){
							elm.className = clsses.replace("" mark"","""");
						} else {
							elm.className += "" mark"";
						}
					}
				</script>
				<style type=""text/css"">
					.green {
						background-color: #00ff00;
					}
					.red {
						background-color: #ff0000;
					}
					.mark {
						background-color: #0000ff;
					}
				</style>";
			string table = String.Format(@"<table><tr><th>Sample</th><th>Command</th><th>Runtime</th><th>Changes (click to show)</th></tr>{0}</table>",Builder.ToString());
			string first = String.Format(@"<p>Report generated for CCExtractor version {0}</p>",data.CCExtractorVersion);

			BuilderDiff.Close ();

			using (StreamWriter sw = new StreamWriter(Path.Combine(pathToFolder,GetReportFileName()))) {
				sw.WriteLine (String.Format (@"
				<html>
					<head>
						<title>{0}</title>
						<style type=""text/css"">{1}</style>
						{2}
					</head>
					<body>", "Report " + DateTime.Now.ToShortDateString (), SideBySideModel.GetCSS (), additionalHeader));
				sw.WriteLine (first);
				sw.WriteLine (table);
				string[] lines = File.ReadAllLines (TempFileName);
				foreach (string line in lines) {
					sw.WriteLine (line);
				}
				sw.WriteLine ("</body></html>");
			}
		}
		#endregion
	}
}
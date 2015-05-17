using System;
using System.IO;
using System.Xml;
using CommandLine;
using CommandLine.Text;

namespace CCExtractorTester
{
	/// <summary>
	/// The Options that are available for use in the command line interface.
	/// </summary>
	class Options {
		[Option('g',"gui",DefaultValue=false,Required=false,HelpText="Use the GUI instead of the CLI")]
		public bool IsGUI { get; set; }
		[Option('t',"test",HelpText="The file that contains a list of the samples to test in xml-format")]
		public string SampleFile { get; set; }
		[Option('c',"config",HelpText="The file that contains the configuration in xml-format")]
		public string ConfigFile { get; set; }
		[Option('d',"debug",DefaultValue=false,Required=false,HelpText="Use debugging")]
		public bool Debug { get; set; }
		[Option('m',"matrix",Required=false,HelpText="Generate a matrix report (features) for all files in this folder")]
		public string Matrix { get; set; }
		[Option('p',"program",DefaultValue=false,Required=false,HelpText="Will the output be parsed by an external program?")]
		public bool IsProgram { get; set; }
		[Option('i',"tempfolder",HelpText="Uses the provided location as a temp folder to store the results in")]
		public string TempFolder { get; set; }
		// Options that will override the config settings
		[Option('e',"executable",HelpText="The CCExtractor executable path (overrides the config file)")]
		public string CCExtractorExecutable { get; set; }
		[Option('r',"reportfolder",HelpText="The path to the folder where the reports will be stored (overrides the config file)")]
		public string ReportFolder { get; set; }
		[Option('s',"samplefolder",HelpText="The path to the folder that contains the samples (overrides the config file)")]
		public string SampleFolder { get; set; }
		[Option('f',"resultfolder",HelpText="The path to the folder that contains the results (overrides the config file)")]
		public string ResultFolder { get; set; }
		[Option('h',"comparer",HelpText="The type of comparer to use  (overrides the config file)")]
		public string Comparer { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption(HelpText="Shows this screen")]
		public string GetUsage(){
			return HelpText.AutoBuild (this, (HelpText current) => HelpText.DefaultParsingErrorsHandler (this, current));
		}
	}

	/// <summary>
	/// Main class of the program.
	/// </summary>
	class MainClass
	{
		/// <summary>
		/// The logger used to log the things that happen while this program runs.
		/// Defaults to the Console + File logger.
		/// </summary>
		public static ILogger Logger = null;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			String location = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			location = location.Remove (location.LastIndexOf (Path.DirectorySeparatorChar));
			Logger = new ConsoleFileLogger (Path.Combine (location, "logs"));
			var options = new Options ();
			if (CommandLine.Parser.Default.ParseArguments (args, options)) {
				Logger.Info ("Starting program - If you encounter any issues using this program, get in touch, and keep this log close to you. (enable CCExtractor output by using -d flag)");
				if (options.Debug) {
					Logger.ActivateDebug ();
					Logger.Debug ("Debug activated");
				}
				if (options.IsGUI) {
					Logger.Debug ("Using GUI - Switching logger to file only");
					Logger = ((ConsoleFileLogger)Logger).FileLogger;
					GUI.Run (Logger);
				} else {
					Logger.Debug ("Using console/command line");
					Logger.Info ("");
					Logger.Info ("If you want to see the usage, run this with --help. Press ctrl-c to abort if necessary");
					Logger.Info ("");
					// Loading configuration
					ConfigurationSettings config = new ConfigurationSettings ();
					if (!String.IsNullOrEmpty (options.ConfigFile) && options.ConfigFile.EndsWith (".xml") && File.Exists (options.ConfigFile)) {
						Logger.Info ("Loading provided configuration ("+options.ConfigFile+")");
						XmlDocument doc = new XmlDocument ();
						doc.Load (options.ConfigFile);
						config = new ConfigurationSettings (doc, options.ConfigFile);
					} else if(!String.IsNullOrEmpty (options.ConfigFile)) {
						Logger.Warn ("Provided config file, but is no xml or does not exist - using default config");
					}
					if (!config.IsAppConfigOK ()) {
						Logger.Error ("Fatal error - config not valid. Please check. Exiting application");
						return;
					}
					String temporaryFolder = Path.Combine (location, "tmpFiles");
					if (!String.IsNullOrEmpty (options.TempFolder) && IsValidDirectory (options.TempFolder)) {
						temporaryFolder = options.TempFolder;
					}
					config.SetAppSetting ("temporaryFolder", temporaryFolder);
					Logger.Info ("Generated result files will be stored in " + temporaryFolder + "(when running multiple tests after another, files might be overwritten)");
					// See what overrides are specified
					if (!String.IsNullOrEmpty (options.CCExtractorExecutable)) {
						if (File.Exists (options.CCExtractorExecutable)) {
							config.SetAppSetting ("CCExtractorLocation", options.CCExtractorExecutable);
							Logger.Info ("Overriding CCExtractorLocation with given version (located at: " + options.CCExtractorExecutable + ")");
						} else {
							Logger.Error ("Given CCExtractor executable path does not exist. Exiting application");
							return;
						}
					}
					if (!String.IsNullOrEmpty (options.Comparer)) {
						config.SetAppSetting ("Comparer", options.Comparer);
						Logger.Info ("Overriding Comparer with: " + options.Comparer);
					}
					if (!String.IsNullOrEmpty (options.ReportFolder)) {
						config.SetAppSetting ("ReportFolder", options.ReportFolder);
						Logger.Info ("Overriding ReportFolder with: " + options.ReportFolder);
					}
					if (!String.IsNullOrEmpty (options.ResultFolder)) {
						config.SetAppSetting ("ResultFolder", options.ResultFolder);
						Logger.Info ("Overriding ResultFolder with: " + options.ResultFolder);
					}
					if (!String.IsNullOrEmpty (options.SampleFolder)) {
						config.SetAppSetting ("SampleFolder", options.SampleFolder);
						Logger.Info ("Overriding SampleFolder with: " + options.SampleFolder);
					}
					// Continue with parameter parsing
					if (!String.IsNullOrEmpty(options.Matrix)) {
						Logger.Info ("Running in report mode, generating matrix");
						if (IsValidDirectory (options.Matrix)) {
							StartMatrixGenerator (options.Matrix, config, Logger);
						} else {
							Logger.Error ("Invalid directory provided for matrix generation!");
						}
					} else if (IsValidPotentialSampleFile (options.SampleFile)) {
						Logger.Info ("Running provided file");
						StartTester (options.SampleFile, config,Logger);					
					} else {
						string sampleFile = config.GetAppSetting ("DefaultTestFile");
						if (IsValidPotentialSampleFile (sampleFile)) {
							Logger.Info ("Running config default file");
							StartTester (sampleFile, config,Logger);
						} else {
							Logger.Error ("No file (or invalid file) provided and default can't be loaded either!");
						}
					}
				}
			}
		}

		/// <summary>
		/// Determines if the provided sampleFile is a valid potential sample file. Checks if it's not empty, has an xml extension and if the file exists.
		/// </summary>
		/// <returns><c>true</c> if the file exists, has an xml extension and the string is not null or empty; otherwise, <c>false</c>.</returns>
		/// <param name="sampleFile">Sample file.</param>
		static bool IsValidPotentialSampleFile (string sampleFile)
		{
			return (!String.IsNullOrEmpty (sampleFile) && sampleFile.EndsWith (".xml") && File.Exists (sampleFile));
		}

		/// <summary>
		/// Determines if the given directory is a valid and existing directory.
		/// </summary>
		/// <returns><c>true</c> if it is a valid and existing directory; otherwise, <c>false</c>.</returns>
		/// <param name="directory">The directory string to check.</param>
		static bool IsValidDirectory (string directory)
		{
			return (!String.IsNullOrEmpty (directory) && Directory.Exists (directory));
		}

		/// <summary>
		/// Starts the tester.
		/// </summary>
		/// <param name="sampleFile">The sample file the tester will be running.</param>
		/// <param name="config">The configuration that will be used by the tester.</param>
		/// <param name="logger">The logger that will be used by the tester.</param>
		static void StartTester (string sampleFile,ConfigurationSettings config,ILogger logger)
		{
			Tester t = new Tester (config,logger, sampleFile);
			t.SetProgressReporter (new ConsoleReporter (logger));
			try {
				t.RunTests ();
			} catch (Exception e) {
				Logger.Error (e);
			}
		}

		static void StartMatrixGenerator (string matrix, ConfigurationSettings config, ILogger logger)
		{
			Reporter r = new Reporter (config, logger, matrix);
			r.SetProgressReporter (new ConsoleReporter (logger));
			try {
				r.GenerateMatrix();
			} catch(Exception e){
				Logger.Error (e);
			};
		}

		// An internal class for logging progress to the console.
		class ConsoleReporter : IProgressReportable {
			private ILogger Logger { get; set; }

			public ConsoleReporter(ILogger logger){
				Logger = logger;
			}

			#region IProgressReportable implementation

			public void showProgressMessage (string message)
			{
				Logger.Info (message);
			}
			#endregion
		}
	}
}
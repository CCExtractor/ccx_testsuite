using System;
using System.IO;
using System.Xml;
using CommandLine;
using CommandLine.Text;

namespace CCExtractorTester
{
	class Options {
		[Option('g',"gui",DefaultValue=false,Required=false,HelpText="Use the GUI instead of the CLI")]
		public bool IsGUI { get; set; }
		[Option('t',"test",HelpText="The file that contains a list of the samples to test in xml-format")]
		public string SampleFile { get; set; }
		[Option('c',"config",HelpText="The file that contains the configuration in xml-format")]
		public string ConfigFile { get; set; }
		[Option('d',"debug",DefaultValue=false,Required=false,HelpText="Use debugging")]
		public bool Debug { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption(HelpText="Shows this screen")]
		public string GetUsage(){
			return HelpText.AutoBuild (this, (HelpText current) => HelpText.DefaultParsingErrorsHandler (this, current));
		}
	}

	// Need mono? http://www.nat.li/linux/how-to-install-mono-2-11-2-on-debian-squeeze
	class MainClass
	{
		public static ILogger Logger = new ConsoleFileLogger ();

		public static void Main (string[] args)
		{
			var options = new Options ();
			if (CommandLine.Parser.Default.ParseArguments (args, options)) {
				Logger.Info ("Starting program - If you encounter any issues using this program, get in touch, and keep this log close to you. (enable CCExtractor output by using -d flag)");
				if (options.Debug) {
					Logger.ActivateDebug ();
					Logger.Debug ("Debug activated");
				}
				if (options.IsGUI) {
					Logger.Debug ("Using GUI - Switching logger to file only");
					Logger = ((ConsoleFileLogger)Logger).Logger;
					GUI.Run (Logger);
				} else {
					Logger.Debug ("Using console/command line");
					Logger.Info ("");
					Logger.Info ("If you want to see the usage, run this with --help. Press ctrl-c to abort if necessary");
					Logger.Info ("");
					ConfigurationSettings config = new ConfigurationSettings ();
					if (!String.IsNullOrEmpty (options.ConfigFile) && options.ConfigFile.EndsWith (".xml") && File.Exists (options.ConfigFile)) {
						Logger.Info ("Loading provided configuration");
						XmlDocument doc = new XmlDocument ();
						doc.Load (args [1]);
						config = new ConfigurationSettings (doc, args [1]);
					} else if(!String.IsNullOrEmpty (options.ConfigFile)) {
						Logger.Warn ("Provided config file, but is no xml or does not exist - using default config");
					}
					if (!config.IsAppConfigOK ()) {
						Logger.Error ("Fatal error - config not valid. Please check. Exiting application");
						return;
					}
					if (IsValidPotentialSampleFile (options.SampleFile)) {
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

		static bool IsValidPotentialSampleFile (string sampleFile)
		{
			return (!String.IsNullOrEmpty (sampleFile) && sampleFile.EndsWith (".xml") && File.Exists (sampleFile));
		}

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
CCExtractorTester
=================

This tool was developed during GSoC 2014 by Willem Van Iseghem.

CCExtractorTester is a testing tool to keep CCExtractor consistent over coding changes. Originally this was (or sometimes still is) achieved by running a set of sample video's on a CCExtractor version, and comparing the generated output to previous stored output (which is known to be correct).

How does it work? It loads a file which contains a video file, the commands for CCExtractor and the location of the correct output file. Then it calls CCExtractor and compares the produced output to the stored output and generates a report for this.

It is written in C# and runs under Mono, in combination with GTK. It can be used command-line or with GUI.

# Requirements

* Mono 2.10 or newer (tested with 2.10.8.1 and 3.3.0), including GtkSharp
* A certain version of CCExtractor
* A set of sample files, together with correct outputs

# Usage

## Windows

### GUI

The GUI can be started by calling the CCExtractor.exe with the "-g" parameter, or use the "RunGUI.bat" file, which will do it for you.

### Commandline

See the parameters below.

## Linux

Need mono? http://www.nat.li/linux/how-to-install-mono-2-11-2-on-debian-squeeze

### GUI

```
#!/bin/bash
exec mono CCExtractorTester.exe -g
```

### Commandline

See the parameters below.

# Command line arguments

  -g, --gui             (Default: False) Use the GUI instead of the CLI
  
  -t, --test            The file that contains a list of the samples to test in xml-format

  -c, --config          The file that contains the configuration in xml-format

  -d, --debug           (Default: False) Use debugging

  -m, --matrix          Generate a matrix report (features) for all files in this folder

  -p, --program         (Default: False) Will the output be parsed by an external program?

  -i, --tempfolder      Uses the provided location as a temp folder to store the results in

  -b, --breakonerror    (Default: False) Will not continue to run more tests once a single error is encountered

  -e, --executable      The CCExtractor executable path (overrides the config file)

  -r, --reportfolder    The path to the folder where the reports will be stored (overrides the config file)

  -s, --samplefolder    The path to the folder that contains the samples (overrides the config file)

  -f, --resultfolder    The path to the folder that contains the results (overrides the config file)

  -h, --comparer        The type of comparer to use (overrides the config file)

  -o, --timeout         (Default: 180) The timeout value (in seconds). A test will be aborted if CCExtractor is still running after this point. Must be bigger than 60.

  --help                Shows this screen

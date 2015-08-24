CCExtractorTester
=================

This tool was developed during GSoC 2014 by Willem Van Iseghem.

CCExtractorTester is a testing tool to keep CCExtractor consistent over coding changes. Originally this was (and sometimes still is) achieved by running a set of sample video's on a certain CCExtractor version (or GitHub commit) and comparing the generated output to previous stored output (which is known to be correct).

The test suite automates this work by loading a file which defines entries that contain a input sample, a list of commands for CCExtractor and a correct output result to compare against. The results are either stored in a report, or can be (starting from 0.8) sent towards a server.

It's written in C# and runs under Mono. It is (starting from 0.8) command-line only.

# Software pre-requisites

## Windows specific

* .NET 4.0 framework or higher

## Linux specific

* Mono 2.10 or newer (A tutorial can be found [here](http://www.nat.li/linux/how-to-install-mono-2-11-2-on-debian-squeeze))

## Common

* CCExtractor in some compiled form
* A set of sample files, together with correct outputs

# Usage

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

CCExtractorTester
=================

This tool was developed during the GSoC 2014 by Willem Van Iseghem.

CCExtractorTester is a testing tool to keep CCExtractor consistent over coding changes. Traditionally this is achieved by running sample video's on a given CCExtractor version, and comparing the generated output to the produced output. Of course, when you have a couple dozen of video files to test each time coding changes are done, it is very time consuming. To solve this particular problem, this test tool was written. It is capable of automatically running CCExtractor, providing it a set of sample files, and automatically compare the generated result with the expected result, and generating a single report with differences.

It is written in C# and runs under Mono, in combination with GTK. It can be used command-line or with GUI.

# Requirements

* Mono 2.10 or newer (tested with 2.10.8.1 and 3.3.0), including GtkSharp
* Some version of CCExtractor
* Some sample files with correct outputs

# Usage

## Windows

### GUI

The GUI can be started by calling the CCExtractor.exe with the "-g" parameter, or use the "RunGUI.bat" file, which will do it for you.

### Commandline

See the parameters below.

## Linux

Need mono? http://www.nat.li/linux/how-to-install-mono-2-11-2-on-debian-squeeze

### GUI

~~Start by calling the~~ Shell script coming soon

### Commandline

See the parameters below.

# Command line arguments

Coming soon

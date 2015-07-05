using System.Reflection;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes.
// Change them to the values specific to your project.
using CommandLine;

[assembly: AssemblyTitle ("CCExtractorTester")]
[assembly: AssemblyDescription ("This program is used to test CCExtractor on correctness of output, based on a sample set of files.")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("CCExtractor")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyCopyright ("Copyright (C) 2014 CCExtractor - developed by Willem Van Iseghem during GSoC 2014 - see LICENSE")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion ("0.7.*")]
// The following attributes are used to specify the signing key for the assembly,
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]
[assembly:AssemblyLicense(
	"This is free software. You may redistribute copies of it under the terms of",
	"the MIT License <http://www.opensource.org/licenses/mit-license.php>.")]
[assembly:AssemblyUsage("Usage:","GUI: CCExtractorTester -g [-d]","Console: CCExtractorTester [-t test.xml] [-c config.xml] [-d]"),"Help: CCExtractortester --help"]
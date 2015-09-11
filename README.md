# CCExtractor test suite

This tool was developed during GSoC 2014 by Willem Van Iseghem.

the CCExtractor test suite is a testing tool to keep CCExtractor consistent over coding changes. Originally this was (and sometimes still is) achieved by running a set of sample video's on a certain CCExtractor version (or GitHub commit) and comparing the generated output to previous stored output (which is known to be correct).

The test suite automates this work by loading a file which defines entries that contain a input sample, a list of commands for CCExtractor and a correct output result to compare against. The results are either stored in a report, or can be (starting from 0.8) sent towards a server.

It's written in C# and runs under Mono. It is (starting from 0.8) command-line only.

## Parameter overview

| Short | Long | Description | Required |
|-------|------|-------------|----------|
| -e | --entries | A XML file containing the test entries | Yes |
| -m | --method | How should the test suite behave for reporting? options: Report (default), Server, Matrix | No |
| -u | --url | If the method is Server, this should point to the url where the suite should send requests to | No |
| -c | --config | A XML file that contains the configuration | No |
| -d | --debug | Enable debugging (extra output) | No |
| -t | --tempfolder | Uses the provided location as a temp folder to store the results in | No |
| -b | --breakonchanges | Break if a change in output (between generated output and correct output) is detected | No |
| | --tcp | Sets the TCP port that will be used in case of entries that need TCP | No |
| | --udp | Sets the UDP port that will be used in case of entries that need UDP| No |
| | --executable | Overrrides the CCExtractor executable path | No |
| | --ffmpeg | Overrrides the FFMpeg executable path | No |
| | --reportfolder | Overrides the folder location where reports will be stored | No |
| | --samplefolder | Overrides the folder location that contains the samples | No |
| | --resultfolder | Overrides the folder location that contains the correct results | No |
| | --comparer | Overrides the type of comparer that will be used | No |
| | --timeout | Overrides the timeout value (default 180 seconds). This indicates how long a single test entry may take to complete. Minimum duration is 60 seconds. | No |
| | --help | Shows this screen | No |

## Usage examples

Generate a matrix report

```
CCExtractortester.exe -m Matrix -e C:\Samples\
```

Run tests for the first generation

```
CCExtractortester.exe -e C:\Samples\SimpleTestFile.xml
```

Run tests for the second generation

```
CCExtractortester.exe -e C:\Samples\MultiTest.xml
```

Run tests for the third generation with local reports

```
CCExtractortester.exe -e C:\Samples\Tests.xml
```

Run tests for the third generation with server reports

```
CCExtractortester.exe -m Server -u http://my.server/report.php -e C:\Samples\Tests.xml
```

## Installation; Software pre-requisites

### Windows specific

* .NET 4.0 framework or higher

### Linux specific

* Mono 2.10 or newer (A tutorial can be found [here](http://www.nat.li/linux/how-to-install-mono-2-11-2-on-debian-squeeze))

### Common

* CommandLineParser NuGet package ([GitHub](https://github.com/gsscoder/commandline), [nuget](https://www.nuget.org/packages/CommandLineParser))
* CCExtractor in some compiled form
* A set of sample files, together with correct outputs

## Changes

See the [changelog](CHANGELOG.md) for version information

## License

The test suite is released under the [MIT License](http://www.opensource.org/licenses/mit-license.php). The license can be found [here](LICENSE).

## Contributing

If you want to help this project forward, or have a solution for some of the issues or bugs, don't hesitate to help! You can fork the project, create a branch for the issue/problem/... and afterwards create a pull request for it.

It will be reviewed as soon as possible.
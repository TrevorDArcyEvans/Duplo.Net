namespace Similitude.UI.CLI;

using CommandLine;
using Core;

internal sealed class Options
{
  [Value(index: 0, Required = true, HelpText = "Path to input file list")]
  public string InputFileList { get; set; }

  [Value(index: 1, Required = true, HelpText = "Path to output file")]
  public string OutputFile { get; set; }

  [Option(longName: "ml", Required = false, HelpText = "minimal block size in lines\n(default is 4)")]
  public int MinimalBlockSize { get; set; } = Duplo.MIN_BLOCK_SIZE;

  [Option(longName: "pt", Required = false, HelpText = "percentage of lines of duplication threshold to override -ml\nUseful for identifying whole file class duplication\n(default is 100%)")]
  public int DuplicateLinesThresholdPercent { get; set; } = 100;

  [Option(longName: "mc", Required = false, HelpText = "minimal characters in line\nLines with less characters are ignored\n(default is 3)")]
  public int MinimalCharsInLine { get; set; } = Duplo.MIN_CHARS;

  [Option(longName: "ip", Required = false, HelpText = "ignore preprocessor directives\n(default is true)")]
  public bool IgnorePreProcessor { get; set; } = true;

  [Option("is", Required = false, HelpText = "ignore file pairs with same name\n(default is true)")]
  public bool IgnoreFilesSameName { get; set; } = true;

  [Option(longName: "xml", Required = false, HelpText = "output file in XML\n(default is true)")]
  public bool OutputXml { get; set; } = true;
}
namespace Duplo.UI.CLI;

using CommandLine;
using Core;

internal sealed class Options
{
  [Value(index: 0, Required = true, HelpText = "Path to input file list")]
  public string InputFileList { get; set; }

  [Value(index: 1, Required = true, HelpText = "Path to output file")]
  public string OutputFile { get; set; }

  [Option('l', longName: "ml", Required = false, Default = 4, HelpText = "minimal block size in lines\n(default is 4)")]
  public int MinimalBlockSize { get; set; }

  [Option('p', "pt", Required = false, Default = 100, HelpText = "percentage of lines of duplication threshold to override -ml\nUseful for identifying whole file class duplication\n(default is 100%)")]
  public int DuplicateLinesThresholdPercent { get; set; }

  [Option('c', "mc", Required = false, Default = 3, HelpText = "minimal characters in line\nLines with less characters are ignored\n(default is 3)")]
  public int MinimalCharsInLine { get; set; }

  [Option('p', longName: "ip", Required = false, Default = false, HelpText = "ignore preprocessor directives\n(default is false)")]
  public bool? IgnorePreProcessor { get; set; }

  [Option('s', "is", Required = false, Default = false, HelpText = "ignore file pairs with same name\n(default is false)")]
  public bool? IgnoreFilesSameName { get; set; }

  [Option('x', longName: "xml", Required = false, Default = false, HelpText = "output file in XML\n(default is false)")]
  public bool? OutputXml { get; set; }
}

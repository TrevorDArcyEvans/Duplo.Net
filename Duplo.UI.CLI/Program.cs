namespace Duplo.UI.CLI;

using CommandLine;
using Core;

internal static class Program
{
  public static void Main(string[] args)
  {
    var result = Parser.Default.ParseArguments<Options>(args)
      .WithParsed(Run);
    result.WithNotParsed(HandleParseError);
  }

  private static void Run(Options opt)
  {
    var duplo = new Duplo(
      opt.InputFileList,
      opt.MinimalBlockSize,
      Clamp(100, 0, opt.DuplicateLinesThresholdPercent),
      opt.MinimalCharsInLine,
      opt.IgnorePreProcessor.Value,
      opt.IgnoreFilesSameName.Value,
      opt.OutputXml.Value);
    duplo.Run(opt.OutputFile);
  }

  private static int Clamp(int upper, int lower, int value)
  {
    return Math.Max(lower, Math.Min(upper, value));
  }

  private static void HandleParseError(IEnumerable<Error> errs)
  {
    if (errs.IsVersion())
    {
      Console.WriteLine($"Duplo {Duplo.VERSION}");
      return;
    }

    if (errs.IsHelp())
    {
      HelpRequest();
      return;
    }

    Console.WriteLine($"Parser Fail");
    return;
  }

  private static void HelpRequest()
  {
    Console.WriteLine($"NAME");
    Console.WriteLine($"       Duplo {Duplo.VERSION} - duplicate source code block finder");
    Console.WriteLine();
    Console.WriteLine($"SYNOPSIS");
    Console.WriteLine($"       duplo [OPTIONS] [INTPUT_FILELIST] [OUTPUT_FILE]");
    Console.WriteLine();
    Console.WriteLine($"DESCRIPTION");
    Console.WriteLine($"       Duplo is a tool to find duplicated code blocks in large");
    Console.WriteLine($"       C/C++/Java/C#/VB.Net software systems.");
    Console.WriteLine();
    Console.WriteLine($"       -ml              minimal block size in lines (default is {Duplo.MIN_BLOCK_SIZE})");
    Console.WriteLine($"       -pt              percentage of lines of duplication threshold to override -ml");
    Console.WriteLine($"                        Useful for identifying whole file class duplication");
    Console.WriteLine($"                        (default is 100%)");
    Console.WriteLine($"       -mc              minimal characters in line. Lines with less characters are ignored");
    Console.WriteLine($"                        (default is {Duplo.MIN_CHARS})");
    Console.WriteLine($"       -ip              ignore preprocessor directives (default is true)");
    Console.WriteLine($"       -is              ignore file pairs with same name (default is true)");
    Console.WriteLine($"       -xml             output file in XML (default is true)");
    Console.WriteLine($"       INTPUT_FILELIST  input filelist");
    Console.WriteLine($"       OUTPUT_FILE      output file");
    Console.WriteLine();
    Console.WriteLine($"VERSION");
    Console.WriteLine($"       {Duplo.VERSION}");
    Console.WriteLine();
    Console.WriteLine($"AUTHORS");
    Console.WriteLine($"       Christian M. Ammann (cammann@giants.ch)");
    Console.WriteLine($"       Trevor D'Arcy-Evans (tdarcyevans@hotmail.com)");
  }
}

namespace Duplo.Core;

using System.Diagnostics;

public sealed class Duplo
{
  public static string VERSION = "0.3.0";
  public static int MIN_BLOCK_SIZE = 4;
  public static int MIN_CHARS = 3;

  private readonly string _listFileName;
  private readonly int _minBlockSize;
  private readonly int _blockPercentThreshold;
  private readonly int _minChars;
  private readonly bool _ignorePrepStuff;
  private readonly bool _ignoreSameFilename;
  private readonly bool _xml;
  private int _maxLinesPerFile = 0;
  private int _duplicateLines = 0;
  //private int[] _matrix;

  public Duplo(
    string listFileName,
    int minBlockSize,
    int blockPercentThreshold,
    int minChars,
    bool ignorePrepStuff,
    bool ignoreSameFilename,
    bool xml,
    int maxLinesPerFile = 0,
    int duplicateLines = 0)
  {
    _listFileName = listFileName;
    _minBlockSize = minBlockSize;
    _blockPercentThreshold = blockPercentThreshold;
    _minChars = minChars;
    _ignorePrepStuff = ignorePrepStuff;
    _ignoreSameFilename = ignoreSameFilename;
    _xml = xml;
    _maxLinesPerFile = maxLinesPerFile;
    _duplicateLines = duplicateLines;
  }

  public void Run(string outputFilePath)
  {
    var outFile = new StreamWriter(outputFilePath);

    if (_xml)
    {
      outFile.WriteLine($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
      outFile.WriteLine($"<?xml-stylesheet href=\"duplo.xsl\" type=\"text/xsl\"?>");
      outFile.WriteLine($"<duplo version=\"{VERSION}\">");
      outFile.WriteLine($"    <check Min_block_size=\"{_minBlockSize}\" Min_char_line=\"{_minChars}\" Ignore_prepro=\"{(_ignorePrepStuff ? "true" : "false")}\" Ignore_same_filename=\"{(_ignoreSameFilename ? "true" : "false")}\">");
    }

    var start = Stopwatch.StartNew();
    Console.WriteLine("Loading and hashing files ... ");

    var listSourceFiles = File
      .ReadLines(_listFileName)
      .Where(str => !string.IsNullOrEmpty(str.Trim()))
      .ToList();
    listSourceFiles.ForEach(str => str.Trim());

    var sourceFiles = new List<SourceFile>();
    var files = 0;
    var locsTotal = 0L;

    // Create vector with all source files
    for (var i = 0; i < listSourceFiles.Count; i++)
    {
      if (listSourceFiles[i].Length > 5)
      {
        var pSourceFile = new SourceFile(listSourceFiles[i], _minChars, _ignorePrepStuff);
        var numLines = pSourceFile.NumberLines;
        if (numLines > 0)
        {
          files++;
          sourceFiles.Add(pSourceFile);
          locsTotal += numLines;
          if (_maxLinesPerFile < numLines)
          {
            _maxLinesPerFile = numLines;
          }
        }
      }
    }

    Console.WriteLine("done.");

    var blocksTotal = 0;

    // Compare each file with each other
    for (var i = 0; i < sourceFiles.Count; i++)
    {
      Console.WriteLine(sourceFiles[i].FilePath);
      var blocks = 0;

      blocks += Process(sourceFiles[i], sourceFiles[i], outFile);
      for (var j = i + 1; j < sourceFiles.Count; j++)
      {
        if ((_ignoreSameFilename && IsSameFilename(sourceFiles[i].FilePath, sourceFiles[j].FilePath)) == false)
        {
          blocks += Process(sourceFiles[i], sourceFiles[j], outFile);
        }
      }

      if (blocks > 0)
      {
        Console.WriteLine($" found: {blocks} block(s)");
      }
      else
      {
        Console.WriteLine(" nothing found.");
      }

      blocksTotal += blocks;
    }


    var duration = start.Elapsed;
    Console.WriteLine($"Time: {duration.Seconds} seconds");

    if (_xml)
    {
      outFile.WriteLine($"        <summary Num_files=\"{files}\" Duplicate_blocks=\"{blocksTotal}\" Total_lines_of_code=\"{locsTotal}\" Duplicate_lines_of_code=\"{_duplicateLines}\" Time=\"{duration.Seconds}\"/>");
      outFile.WriteLine($"    </check>");
      outFile.WriteLine($"</duplo>");
    }
    else
    {
      outFile.WriteLine($"Configuration: ");
      outFile.WriteLine($"  Number of files: {files}");
      outFile.WriteLine($"  Minimal block size: {_minBlockSize}");
      outFile.WriteLine($"  Minimal characters in line: {_minChars}");
      outFile.WriteLine($"  Ignore preprocessor directives: {_ignorePrepStuff}");
      outFile.WriteLine($"  Ignore same filenames: {_ignoreSameFilename}");
      outFile.WriteLine();
      outFile.WriteLine($"Results: ");
      outFile.WriteLine($"  Lines of code: {locsTotal}");
      outFile.WriteLine($"  Duplicate lines of code: {_duplicateLines}");
      outFile.WriteLine($"  Total {blocksTotal} duplicate block(s) found.");
      outFile.WriteLine($"  Time: {duration.Seconds} seconds");
    }
  }

  private void ReportSeq(int line1, int line2, int count, SourceFile pSource1, SourceFile pSource2, TextWriter outFile)
  {
    if (_xml)
    {
      outFile.WriteLine($"    <set LineCount=\"{count}\">");
      outFile.WriteLine($"        <block SourceFile=\"{pSource1.FilePath}\" StartLineNumber=\"{pSource1.Line(line1).LineNumber}\"/>");
      outFile.WriteLine($"        <block SourceFile=\"{pSource2.FilePath}\" StartLineNumber=\"{pSource2.Line(line2).LineNumber}\"/>");
      outFile.WriteLine($"        <lines xml:space=\"preserve\">");
      for (var j = 0; j < count; j++)
      {
        // replace various characters/ strings so that it doesn't upset the XML parser
        var tmpstr = pSource1.Line(j + line1).Line;

        // " --> '
        tmpstr = tmpstr.Replace("\'", "\"");

        // & --> &amp;
        tmpstr = tmpstr.Replace("&amp;", "&");

        // < --> &lt;
        tmpstr = tmpstr.Replace("&lt;", "<");

        // > --> &gt;
        tmpstr = tmpstr.Replace("&gt;", ">");

        outFile.WriteLine($"            <line Text=\"{tmpstr}\"/>");
        _duplicateLines++;
      }

      outFile.WriteLine("        </lines>");
      outFile.WriteLine("    </set>");
    }
    else
    {
      outFile.WriteLine($"{pSource1.FilePath} ({pSource1.Line(line1).LineNumber})");
      outFile.WriteLine($"{pSource2.FilePath} ({pSource2.Line(line2).LineNumber})");
      for (var j = 0; j < count; j++)
      {
        outFile.WriteLine($"{pSource1.Line(j + line1).Line}");
        _duplicateLines++;
      }

      outFile.WriteLine();
    }
  }

  private int Process(SourceFile pSource1, SourceFile pSource2, TextWriter outFile)
  {
    // Generate matrix large enough for all files
    var matrix = new bool[_maxLinesPerFile * _maxLinesPerFile];

    var m = pSource1.NumberLines;
    var n = pSource2.NumberLines;

    // Compute matrix
    for (var y = 0; y < m; y++)
    {
      var pSLine = pSource1.Line(y);
      for (var x = 0; x < n; x++)
      {
        if (pSLine.Hash == pSource2.Line(x).Hash)
        {
          matrix[x + n * y] = true;
        }
      }
    }

    // support reporting filtering by both:
    // - "lines of code duplicated", &
    // - "percentage of file duplicated"
    var lMinBlockSize = Math.Max(
      _minBlockSize, Math.Min(
        _minBlockSize,
        (Math.Max(n, m) * 100) / _blockPercentThreshold));

    var blocks = 0;

    // Scan vertical part
    for (var y = 0; y < m; y++)
    {
      var seqLen = 0;
      var maxX = Math.Min(n, m - y);
      for (var x = 0; x < maxX; x++)
      {
        if (matrix[x + n * (y + x)])
        {
          seqLen++;
        }
        else
        {
          if (seqLen >= lMinBlockSize)
          {
            var line1 = y + x - seqLen;
            var line2 = x - seqLen;
            if (!((line1 == line2) && (pSource1 == pSource2)))
            {
              ReportSeq(line1, line2, seqLen, pSource1, pSource2, outFile);
              blocks++;
            }
          }

          seqLen = 0;
        }
      }

      if (seqLen >= lMinBlockSize)
      {
        var line1 = m - seqLen;
        var line2 = n - seqLen;
        if (!((line1 == line2) && (pSource1 == pSource2)))
        {
          ReportSeq(line1, line2, seqLen, pSource1, pSource2, outFile);
          blocks++;
        }
      }
    }

    if (pSource1 != pSource2)
    {
      // Scan horizontal part
      for (var x = 1; x < n; x++)
      {
        var seqLen = 0;
        var maxY = Math.Min(m, n - x);
        for (var y = 0; y < maxY; y++)
        {
          if (matrix[x + y + n * y])
          {
            seqLen++;
          }
          else
          {
            if (seqLen >= lMinBlockSize)
            {
              ReportSeq(y - seqLen, x + y - seqLen, seqLen, pSource1, pSource2, outFile);
              blocks++;
            }

            seqLen = 0;
          }
        }

        if (seqLen >= lMinBlockSize)
        {
          ReportSeq(m - seqLen, n - seqLen, seqLen, pSource1, pSource2, outFile);
          blocks++;
        }
      }
    }

    return blocks;
  }

  private static bool IsSameFilename(string filename1, string filename2)
  {
    return Path.GetFileName(filename1) == Path.GetFileName(filename2);
  }
}

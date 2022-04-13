namespace Similitude.Core;

public sealed class Duplo
{
  public static string VERSION = "0.3.0";
  public static int MIN_BLOCK_SIZE = 4;
  public static int MIN_CHARS = 3;

  string m_listFileName;
  int m_minBlockSize;
  int m_blockPercentThreshold;
  int m_minChars;
  bool m_ignorePrepStuff;
  bool m_ignoreSameFilename;
  bool m_Xml;
  int m_maxLinesPerFile = 0;
  int m_DuplicateLines = 0;
  char[] m_pMatrix;

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
    m_listFileName = listFileName;
    m_minBlockSize = minBlockSize;
    m_blockPercentThreshold = blockPercentThreshold;
    m_minChars = minChars;
    m_ignorePrepStuff = ignorePrepStuff;
    m_ignoreSameFilename = ignoreSameFilename;
    m_Xml = xml;
    m_maxLinesPerFile = maxLinesPerFile;
    m_DuplicateLines = duplicateLines;
  }

  public void Run(string outputFilePath)
  {
  }

  private void reportSeq(int line1, int line2, int count, SourceFile pSource1, SourceFile pSource2, TextWriter outFile)
  {
    if (m_Xml)
    {
      outFile.WriteLine($"    <set LineCount=\"{count}\">");
      outFile.WriteLine($"        <block SourceFile=\"{pSource1.FilePath}\" StartLineNumber=\"{pSource1.Line(line1).LineNumber}\"/>");
      outFile.WriteLine($"        <block SourceFile=\"{pSource2.FilePath}\" StartLineNumber=\"{pSource2.Line(line2).LineNumber}\"/>");
      outFile.WriteLine($"        <lines xml:space=\"preserve\">");
      for (int j = 0; j < count; j++)
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
        m_DuplicateLines++;
      }

      outFile.WriteLine("        </lines>");
      outFile.WriteLine("    </set>");
    }
    else
    {
      outFile.WriteLine($"{pSource1.FilePath} ({pSource1.Line(line1).LineNumber})");
      outFile.WriteLine($"{pSource2.FilePath} ({pSource2.Line(line2).LineNumber})");
      for (int j = 0; j < count; j++)
      {
        outFile.WriteLine($"{pSource1.Line(j + line1).Line}");
        m_DuplicateLines++;
      }

      outFile.WriteLine();
    }
  }

  private static bool IsSameFilename(string filename1, string filename2)
  {
    return Path.GetFileName(filename1) == Path.GetFileName(filename2);
  }

  private static int Clamp(int upper, int lower, int value)
  {
    return Math.Max(lower, Math.Min(upper, value));
  }
}

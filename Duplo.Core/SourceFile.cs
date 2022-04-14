namespace Duplo.Core;

using System.Text;

public sealed class SourceFile
{
  public string FilePath { get; }
  public int NumberLines => _sourceLines.Count;
  public SourceLine Line(int index) => _sourceLines[index];

  private readonly FileType _fileType;
  private readonly int _minChars;
  private readonly bool _ignorePrepStuff;
  private readonly List<SourceLine> _sourceLines = new();

  public SourceFile(string filePath, int minChars, bool ignorePrepStuff)
  {
    FilePath = filePath;
    _fileType = FileTypeUtils.GetFileType(filePath);
    _minChars = minChars;
    _ignorePrepStuff = ignorePrepStuff;

    var lines = File
      .ReadLines(FilePath)
      .Where(str => !string.IsNullOrEmpty(str.Trim()))
      .ToList();
    var openBlockComments = 0;
    for (var i = 0; i < lines.Count(); i++)
    {
      var line = lines[i];
      var tmp = new StringBuilder();

      switch (_fileType)
      {
        // Remove block comments
        case FileType.FILETYPE_C:
        case FileType.FILETYPE_CPP:
        case FileType.FILETYPE_CXX:
        case FileType.FILETYPE_H:
        case FileType.FILETYPE_HPP:
        case FileType.FILETYPE_JAVA:
        case FileType.FILETYPE_CS:
        {
          var lineSize = line.Length;
          for (var j = 0; j < (int)line.Length; j++)
          {
            if (line[j] == '/' && line[Math.Min(lineSize - 1, j + 1)] == '*')
            {
              openBlockComments++;
            }

            if (openBlockComments <= 0)
            {
              tmp.Append(line[j]);
            }

            if (line[Math.Max(0, j - 1)] == '*' && line[j] == '/')
            {
              openBlockComments--;
            }
          }

          break;
        }

        case FileType.FILETYPE_VB:
        case FileType.FILETYPE_UNKNOWN:
        {
          tmp.Append(line);
          break;
        }

        case FileType.FILETYPE_S:
        {
          var lineToEolMarker = line.Substring(0, line.IndexOf(";") - 1);
          tmp.Append(lineToEolMarker);
          break;
        }
      }

      var cleaned = CleanLine(tmp.ToString());
      if (IsSourceLine(cleaned))
      {
        _sourceLines.Add(new SourceLine(cleaned, i));
      }
    }
  }

  private string CleanLine(string line)
  {
    var cleanedLine = new StringBuilder();
    var lineSize = line.Length;
    for (var i = 0; i < line.Length; i++)
    {
      switch (_fileType)
      {
        case FileType.FILETYPE_C:
        case FileType.FILETYPE_CPP:
        case FileType.FILETYPE_CXX:
        case FileType.FILETYPE_H:
        case FileType.FILETYPE_HPP:
        case FileType.FILETYPE_JAVA:
        case FileType.FILETYPE_CS:
          if (i < lineSize - 2 && line[i] == '/' && line[i + 1] == '/')
          {
            return cleanedLine.ToString();
          }

          break;

        case FileType.FILETYPE_VB:
          if (i < lineSize - 1 && line[i] == '\'')
          {
            return cleanedLine.ToString();
          }

          break;

        case FileType.FILETYPE_S:
          if (i < lineSize - 1 && line[i] == ';')
          {
            return cleanedLine.ToString();
          }

          break;

        // no pre-processing of code of unknown languages
        case FileType.FILETYPE_UNKNOWN:
          break;
      }

      cleanedLine.Append(line[i]);
    }

    return cleanedLine.ToString();
  }

  private bool IsSourceLine(string line)
  {
    var tmp = line.Trim().ToLowerInvariant();

    // filter min size lines
    if (tmp.Length < _minChars)
    {
      return false;
    }

    if (_ignorePrepStuff)
    {
      switch (_fileType)
      {
        case FileType.FILETYPE_C:
        case FileType.FILETYPE_CPP:
        case FileType.FILETYPE_CXX:
        case FileType.FILETYPE_H:
        case FileType.FILETYPE_HPP:
        case FileType.FILETYPE_JAVA:
          if (tmp[0] == '#')
          {
            return false;
          }

          break;

        case FileType.FILETYPE_CS:
        {
          if (tmp[0] == '#')
          {
            return false;
          }

          // look for attribute
          if (tmp[0] == '[')
          {
            return false;
          }

          // look for other markers to avoid
          string[] PreProc_CS = { "namespace", "using", "private", "protected", "public" };
          foreach (var preProc in PreProc_CS)
          {
            if (tmp.StartsWith(preProc))
            {
              return false;
            }
          }
        }
          break;

        case FileType.FILETYPE_VB:
        {
          // look for preprocessor marker in start of string
          string[] PreProc_VB = { "imports" };
          foreach (var preProc in PreProc_VB)
          {
            if (tmp.StartsWith(preProc))
            {
              return false;
            }
          }
        }
          break;

        case FileType.FILETYPE_S:
        {
          string[] PreProc_S = { "ret" }; //we can't deduplicate ret AFAIK
          foreach (var preProc in PreProc_S)
          {
            if (tmp.StartsWith(preProc))
            {
              return false;
            }
          }
        }
          break;

        // no pre-processing of code of unknown languages
        case FileType.FILETYPE_UNKNOWN:
          break;
      }
    }

    // must be at least one alpha-numeric character
    return tmp.Length >= _minChars && tmp.Any(char.IsLetterOrDigit);
  }

  #region Equality

  private bool Equals(SourceFile other)
  {
    return FilePath == other.FilePath;
  }

  public override bool Equals(object? obj)
  {
    return ReferenceEquals(this, obj) || obj is SourceFile other && Equals(other);
  }

  public override int GetHashCode()
  {
    return FilePath.GetHashCode();
  }

  public static bool operator ==(SourceFile? left, SourceFile? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(SourceFile? left, SourceFile? right)
  {
    return !Equals(left, right);
  }

  #endregion
}

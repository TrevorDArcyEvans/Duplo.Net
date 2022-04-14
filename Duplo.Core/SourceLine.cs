namespace Duplo.Core;

public sealed class SourceLine
{
  public string Line { get; }
  public int LineNumber { get; }

  public string Hash { get; }

  public SourceLine(string line, int lineNumber)
  {
    Line = line;
    LineNumber = lineNumber;

    var cleanLine = Line
      .Replace(" ", string.Empty)
      .Replace("\t", string.Empty);
    Hash = CreateMD5(cleanLine);
  }

  private static string CreateMD5(string input)
  {
    // Use input string to calculate MD5 hash
    using var md5 = System.Security.Cryptography.MD5.Create();
    var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
    var hashBytes = md5.ComputeHash(inputBytes);

    return Convert.ToHexString(hashBytes); // .NET 5 +
  }

  #region Equality

  private bool Equals(SourceLine other)
  {
    return Hash == other.Hash;
  }

  public override bool Equals(object? obj)
  {
    return ReferenceEquals(this, obj) || obj is SourceLine other && Equals(other);
  }

  public override int GetHashCode()
  {
    return Hash.GetHashCode();
  }

  public static bool operator ==(SourceLine? left, SourceLine? right)
  {
    return Equals(left, right);
  }

  public static bool operator !=(SourceLine? left, SourceLine? right)
  {
    return !Equals(left, right);
  }

  #endregion
}
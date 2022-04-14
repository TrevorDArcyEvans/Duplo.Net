namespace Similitude.Core;

public sealed class FileTypeUtils
{
  public const string FileTypeExtn_C = "c";
  public const string FileTypeExtn_CPP = "cpp";
  public const string FileTypeExtn_CXX = "cxx";
  public const string FileTypeExtn_H = "h";
  public const string FileTypeExtn_HPP = "hpp";
  public const string FileTypeExtn_Java = "java";
  public const string FileTypeExtn_CS = "cs";
  public const string FileTypeExtn_VB = "vb";
  public const string FileTypeExtn_S = "s"; // gcc assembly

  public static FileType GetFileType(string filePath)
  {
    // get lower case file extension
    var fileExtn = Path.GetExtension(filePath).ToLowerInvariant();

    return fileExtn switch
    {
      FileTypeExtn_C => FileType.FILETYPE_C,
      FileTypeExtn_CPP => FileType.FILETYPE_CPP,
      FileTypeExtn_CXX => FileType.FILETYPE_CXX,
      FileTypeExtn_H => FileType.FILETYPE_H,
      FileTypeExtn_HPP => FileType.FILETYPE_HPP,
      FileTypeExtn_Java => FileType.FILETYPE_JAVA,
      FileTypeExtn_CS => FileType.FILETYPE_CS,
      FileTypeExtn_VB => FileType.FILETYPE_VB,
      FileTypeExtn_S => FileType.FILETYPE_S,
      _ => FileType.FILETYPE_UNKNOWN
    };
  }
}

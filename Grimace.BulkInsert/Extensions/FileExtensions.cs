using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Grimace.BulkInsert.Extensions
{
  public static class FileExtensions
  {
    public static string CreateLink(this string path)
    {
      return "file://" + path.Replace('\\', '/');
    }

    public static string CreateLinkFromContent(this string content, string extension)
    {
      var tempPath = GetTempFilePath(extension);
      File.WriteAllText(tempPath, content, Encoding.Unicode);
      return tempPath.CreateLink();
    }

    public static string GetTempFilePath(this string extension)
    {
      string fullPath;
      do
      {
        var path = Path.GetTempPath();
        var fileName = Guid.NewGuid() + extension;

        fullPath = Path.Combine(path, fileName);
      }
      while (File.Exists(fullPath));

      return fullPath;
    }
  }
}

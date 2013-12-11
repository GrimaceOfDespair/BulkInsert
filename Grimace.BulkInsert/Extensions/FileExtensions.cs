#region Copyright Notice
// This file is part of Grimace.BulkInsert.
// Bulk Insert into SQL without files
// Copyright (C) 2013 Grimace of Despair
// 
// Grimace.BulkInsert is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Grimace.BulkInsert is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

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

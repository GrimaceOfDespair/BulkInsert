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
using System.Xml.Serialization;
using Grimace.BulkInsert.FormatFile;

namespace Grimace.BulkInsert.Extensions
{
  public static class TypeExtensions
  {
    public static T DbToClr<T>(this object obj, T defaultValue = default(T))
    {
      return
        obj == null || obj == DBNull.Value
          ? defaultValue
          : (T) obj;
    }

    public static string SerializeToXml<T>(this T @object)
    {
      return SerializeToXml(@object, typeof (T));
    }

    public static string SerializeToXml(this object @object, Type type)
    {
      var serializer = new XmlSerializer(type);
      using (var formatWriter = new StringWriter())
      {
        serializer.Serialize(formatWriter, @object);

        return formatWriter.ToString();
      }
    }

    public static T DeserializeFromXml<T>(this string xml)
    {
      var serializer = new XmlSerializer(typeof(T));
      using (var formatReader = new StringReader(xml))
      {
        return (T)serializer.Deserialize(formatReader);
      }
    }
  }
}

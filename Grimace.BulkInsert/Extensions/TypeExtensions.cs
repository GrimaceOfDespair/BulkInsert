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

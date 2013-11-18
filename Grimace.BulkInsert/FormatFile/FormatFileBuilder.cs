using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Grimace.BulkInsert.Extensions;
using Grimace.BulkInsert.Resources;
using Grimace.BulkInsert.Schema;
using log4net;

namespace Grimace.BulkInsert.FormatFile
{
  public class FormatFileBuilder
  {
    public static string GetFormatXml(IEnumerable<DbColumn> dbColumns)
    {
      var formatType = CreateFormatType(dbColumns);

      var serializer = new XmlSerializer(typeof (bcpFormatType));
      using (var formatWriter = new StringWriter())
      {
        serializer.Serialize(formatWriter, formatType);

        return formatWriter.ToString();
      }
    }

    public static string CreateFormatFile(IEnumerable<DbColumn> dbColumns)
    {
      var formatXml = GetFormatXml(dbColumns);
      var xmlFile = ".xml".GetTempFilePath();

      File.WriteAllText(xmlFile, formatXml, Encoding.Unicode);

      return xmlFile;
    }

    public static bcpFormatType CreateFormatType(IEnumerable<DbColumn> colums)
    {
      var fieldTypes = new List<AnyFieldType>();
      var columntypes = new List<AnyColumnType>();

      foreach (var column in colums)
      {
        fieldTypes.Add(CreateFieldDescriptor(column));
        columntypes.Add(CreateColumnDescriptor(column));
      }

      var last = fieldTypes.Last() as CharTerm;
      if (last != null)
      {
        last.TERMINATOR = "\\0\\0";
      }

      return new bcpFormatType
               {
                 RECORD = fieldTypes.ToArray(),
                 ROW = columntypes.ToArray(),
               };
    }

    private static AnyColumnType CreateColumnDescriptor(DbColumn column)
    {
      var columnTypeString = typeof(bcpFormatType).Namespace + ".SQL" + column.SqlType.ToUpperInvariant();
      var columntype = Type.GetType(columnTypeString);
      if (columntype == null)
      {
        throw new FormatFileException(string.Format(Strings.InvalidColumnType, columnTypeString));
      }

      var columnType = (AnyColumnType) Activator.CreateInstance(columntype);

      AnyColumnTypeNULLABLE nullable;
      Enum.TryParse(column.Nullable, out nullable);

      columnType.NAME = column.Name;
      columnType.NULLABLE = nullable;
      columnType.SOURCE = column.Id;

      return columnType;
    }

    private static AnyFieldType CreateFieldDescriptor(DbColumn column)
    {
      return new NCharTerm
               {
                 ID = column.Id,
                 TERMINATOR = "\\0",
                 MAX_LENGTH = column.MaxLength.ToString(CultureInfo.InvariantCulture),
               };
    }
  }
}

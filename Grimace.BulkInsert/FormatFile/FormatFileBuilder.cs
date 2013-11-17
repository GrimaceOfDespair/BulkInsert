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
using Grimace.BulkInsert.Resources;
using Grimace.BulkInsert.Schema;

namespace Grimace.BulkInsert.FormatFile
{
  public class FormatFileBuilder
  {
    public SchemaProvider SchemaProvider { get; private set; }

    public FormatFileBuilder(SchemaProvider schemaProvider)
    {
      SchemaProvider = schemaProvider;
    }

    public string GetFormatXml(IEnumerable<DbColumn> dbColumns)
    {
      var schema = CreateSchema(dbColumns);

      var serializer = new XmlSerializer(typeof (bcpFormatType));
      var schemaWriter = new StringWriter();
      serializer.Serialize(schemaWriter, schema);

      return schemaWriter.ToString();
    }

    public bcpFormatType CreateSchema(IEnumerable<DbColumn> colums)
    {
      var fieldTypes = new List<AnyFieldType>();
      var columntypes = new List<AnyColumnType>();

      foreach (var column in colums)
      {
        fieldTypes.Add(CreateFieldDescriptor(column));
        columntypes.Add(CreateColumnDescriptor(column));
      }

      return new bcpFormatType
               {
                 RECORD = fieldTypes.ToArray(),
                 ROW = columntypes.ToArray(),
               };
    }

    private AnyColumnType CreateColumnDescriptor(DbColumn column)
    {
      var columnTypeString = "Grimace.BulkInsert.FormatFile.SQL" + column.SqlType.ToUpperInvariant();
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

    private AnyFieldType CreateFieldDescriptor(DbColumn column)
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

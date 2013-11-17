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

namespace Grimace.BulkInsert.FormatFile
{
  public class FormatFileBuilder
  {
    public DbConnection DbConnection { get; private set; }

    public FormatFileBuilder(DbConnection dbConnection)
    {
      DbConnection = dbConnection;
    }

    public string CreateXml(string tableName, string[] columnNames)
    {
      var dbColumns = GetColumns(tableName);

      var schema = CreateSchema(dbColumns);

      var serializer = new XmlSerializer(typeof (bcpFormatType));
      var schemaWriter = new StringWriter();
      serializer.Serialize(schemaWriter, schema);

      return schemaWriter.ToString();
    }

    public IEnumerable<DbColumn> GetColumns(string tableName)
    {
      using (var selectColumnsCommand = DbConnection.CreateCommand())
      {
        selectColumnsCommand.CommandText = string.Format(
          "SELECT column_name, data_type, is_nullable, character_maximum_length " +
          "FROM information_schema.COLUMNS " +
          "WHERE table_name='{0}'",
          tableName);

        using (var columnReader = selectColumnsCommand.ExecuteReader())
        {
          var id = 1;

          while (columnReader.Read())
          {
            var sqlType = (string)columnReader["data_type"] ?? "";

            var maxLength = (columnReader["character_maximum_length"] as int?) ?? 0;
            if (maxLength == 0)
            {
              maxLength = DbTypes.GetMaxLength(sqlType);
            }

            yield return new DbColumn
                           {
                             Id = id++.ToString(CultureInfo.InvariantCulture),
                             Name = (string) columnReader["column_name"] ?? "",
                             SqlType = sqlType,
                             Nullable = (string) columnReader["is_nullable"] ?? "",
                             MaxLength = maxLength
                           };
          }
        }
      }
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

    public struct DbColumn
    {
      public string Id;
      public string Name;
      public string SqlType;
      public string Nullable;
      public int MaxLength;
    }
  }
}

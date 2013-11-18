using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using Grimace.BulkInsert.Extensions;
using Grimace.BulkInsert.FormatFile;

namespace Grimace.BulkInsert.Schema
{
  public class SchemaProvider : ISchemaProvider
  {
    private readonly DbConnection _sqlConnection;

    public SchemaProvider(DbConnection sqlConnection)
    {
      _sqlConnection = sqlConnection;
    }

    public IEnumerable<DbColumn> GetColumns(string tableName)
    {
      using (var selectColumnsCommand = _sqlConnection.CreateCommand())
      {
        selectColumnsCommand.CommandText = string.Format(
          "SELECT column_name, data_type, is_nullable, character_maximum_length, collation_name " +
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

            var dbColumn = new DbColumn
                             {
                               Id = id++.ToString(CultureInfo.InvariantCulture),
                               Name = columnReader["column_name"].DbToClr(""),
                               SqlType = sqlType,
                               Nullable = columnReader["is_nullable"].DbToClr(""),
                               MaxLength = maxLength,
                               CollationName = columnReader["collation_name"].DbToClr(""),
                             };

            yield return dbColumn;
          }
        }
      }
    }
  }
}
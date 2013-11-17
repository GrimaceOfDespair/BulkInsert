using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
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

            var dbColumn = new DbColumn
                             {
                               Id = id++.ToString(CultureInfo.InvariantCulture),
                               Name = (string) columnReader["column_name"] ?? "",
                               SqlType = sqlType,
                               Nullable = (string) columnReader["is_nullable"] ?? "",
                               MaxLength = maxLength
                             };

            if (Suppress(dbColumn) == false)
            {
              yield return dbColumn;
            }

          }
        }
      }
    }

    protected virtual bool Suppress(DbColumn dbColumn)
    {
      return dbColumn.Id.Equals("id", StringComparison.CurrentCultureIgnoreCase);
    }

    //public IEnumerable<DbColumn> GetPrimaryKeys(string tableName)
    //{
    //  using (var selectColumnsCommand = DbConnection.CreateCommand())
    //  {
    //    selectColumnsCommand.CommandText = string.Format(
    //      "SELECT c.column_name, c.data_type, c.is_nullable, c.character_maximum_length " +
    //      "FROM information_schema.COLUMNS c" +
    //      "LEFT JOIN (" +
    //        "SELECT ku.TABLE_CATALOG,ku.TABLE_SCHEMA,ku.TABLE_NAME,ku.COLUMN_NAME " +
    //        "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc " +
    //        "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku" +
    //          "ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' " +
    //          "AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME" +
    //      ") pk " +
    //      "ON c.TABLE_CATALOG = pk.TABLE_CATALOG " +
    //        "AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA " +
    //        "AND c.TABLE_NAME = pk.TABLE_NAME " +
    //        "AND c.COLUMN_NAME = pk.COLUMN_NAME" +
    //      "WHERE c.table_name='{0}'",
    //      tableName);

    //    using (var columnReader = selectColumnsCommand.ExecuteReader())
    //    {
    //      var id = 1;

    //      while (columnReader.Read())
    //      {
    //      }
    //    }
    //  }
    //}

  }
}
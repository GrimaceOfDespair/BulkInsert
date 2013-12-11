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
            var charWidth = 1;

            var sqlType = (string)columnReader["data_type"] ?? "";

            var maxLength = (columnReader["character_maximum_length"] as int?) ?? 0;
            if (maxLength == 0)
            {
              maxLength = DbTypes.GetMaxLength(sqlType);
            }
            else
            {
              charWidth = DbTypes.GetCharWidth(sqlType);
            }

            var dbColumn = new DbColumn
                             {
                               Id = id++.ToString(CultureInfo.InvariantCulture),
                               Name = columnReader["column_name"].DbToClr(""),
                               SqlType = sqlType,
                               Nullable = columnReader["is_nullable"].DbToClr(""),
                               MaxLength = maxLength * charWidth,
                               CollationName = columnReader["collation_name"].DbToClr(""),
                             };

            yield return dbColumn;
          }
        }
      }
    }
  }
}
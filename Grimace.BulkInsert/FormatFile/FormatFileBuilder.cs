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
      return CreateFormatType(dbColumns).SerializeToXml();
    }

    public static string CreateFormatFile(IEnumerable<DbColumn> dbColumns)
    {
      var formatXml = GetFormatXml(dbColumns);
      var xmlFile = ".xml".GetTempFilePath();

      File.WriteAllText(xmlFile, formatXml, Encoding.Unicode);

      return xmlFile;
    }

    public static bcpFormatType CreateFormatType(IEnumerable<DbColumn> colums, bool outputNullability = false)
    {
      var fieldTypes = new List<AnyFieldType>();
      var columntypes = new List<AnyColumnType>();

      foreach (var column in colums)
      {
        fieldTypes.Add(CreateFieldDescriptor(column));
        columntypes.Add(CreateColumnDescriptor(column, outputNullability));
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

    private static AnyColumnType CreateColumnDescriptor(DbColumn dbColumn, bool outputNullability)
    {
      var sqlType = dbColumn.SqlType.ToUpperInvariant();
      switch (sqlType)
      {
        case "VARCHAR":
          sqlType = "VARYCHAR";
          break;
        case "VARBIN":
          sqlType = "VARYBIN";
          break;
        case "SMALLDATETIME":
          sqlType = "DATETIM4";
          break;
      }

      var columnTypeString = typeof(bcpFormatType).Namespace + ".SQL" + sqlType;

      var columntype = Type.GetType(columnTypeString);
      if (columntype == null)
      {
        throw new FormatFileException(string.Format(Strings.InvalidColumnType, columnTypeString));
      }

      var columnType = (AnyColumnType) Activator.CreateInstance(columntype);

      columnType.NAME = dbColumn.Name;
      columnType.SOURCE = dbColumn.Id;

      if (outputNullability)
      {
        AnyColumnTypeNULLABLE nullable;
        Enum.TryParse(dbColumn.Nullable, out nullable);
        columnType.NULLABLE = nullable;
      }

      return columnType;
    }

    private static AnyFieldType CreateFieldDescriptor(DbColumn dbColumn)
    {
      var fieldDescriptor = new CharTerm
                        {
                          ID = dbColumn.Id,
                          TERMINATOR = "\\0",
                          MAX_LENGTH = dbColumn.MaxLength.ToString(CultureInfo.InvariantCulture),
                        };

      if (string.IsNullOrEmpty(dbColumn.CollationName) == false)
      {
        fieldDescriptor.COLLATION = dbColumn.CollationName;
      }

      return fieldDescriptor;
    }
  }
}

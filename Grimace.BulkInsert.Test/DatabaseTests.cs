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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Grimace.BulkInsert.Test
{
  public class DatabaseTests
  {
    public SqlConnection SqlConnection;

    public Regex FixedCharType = new Regex(@"(?<!var)char\((?<size>\d+)\)", RegexOptions.IgnoreCase);

    public string[] ColumnTypes = new[]
                                    {
                                      "BigInt",
                                      "Binary",
                                      "Bit",
                                      "Char",
                                      "DateTime",
                                      "Decimal(1,0)",
                                      "Decimal(18,0)",
                                      "Decimal(38,0)",
                                      "Float",
                                      "Image",
                                      "Int",
                                      "Money",
                                      "NChar",
                                      "NText",
                                      "Numeric(1,0)",
                                      "Numeric(18,0)",
                                      "Numeric(38,0)",
                                      "NVarChar(1)",
                                      "NVarChar(255)",
                                      "NVarChar(4096)",
                                      "Real",
                                      "UniqueIdentifier",
                                      "SmallDateTime",
                                      "SmallInt",
                                      "SmallMoney",
                                      "Text",
                                      "Timestamp",
                                      "TinyInt",
                                      "VarBinary",
                                      "VarChar(1)",
                                      "VarChar(255)",
                                      "VarChar(4096)",
                                      "Variant",
                                      "Xml",
                                      "Udt",
                                      "Structured",
                                      "Date",
                                      "Time",
                                      "DateTime2",
                                      "DateTimeOffset",
                                    };


    [TestFixtureSetUp]
    public void Initialize()
    {
      AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
      var connectionString = ConfigurationManager.ConnectionStrings["SampleData"].ConnectionString;
      SqlConnection = new SqlConnection(connectionString);
      SqlConnection.Open();
    }

    [TestFixtureTearDown]
    public void Dispose()
    {
      SqlConnection.Dispose();
    }

    public void CreateTable(string tableName, params string[] columnTypes)
    {
      var createTableCommand = SqlConnection.CreateCommand();
      createTableCommand.CommandText =

        string.Format(

          "IF OBJECT_ID('{0}', 'U') IS NULL " +
          "BEGIN " +
            "CREATE TABLE [{0}] " +
            "(" +
              "Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY," +
              "{1}" +
            ") " +
          "END " +
          "ELSE " +
          " TRUNCATE TABLE [{0}]",

          tableName,

          string.Join(",",
                      columnTypes
                        .Select(t =>
                                string.Format("[{0}] {1}", t, t))
                        .ToArray()));

      Console.WriteLine(createTableCommand.CommandText);

      createTableCommand.ExecuteNonQuery();
    }

    protected void VerifyRows(string tableName, string[] columnNames, IEnumerable<string[]> dataValues, string dataFormat = null)
    {
      var selectCommand = SqlConnection.CreateCommand();
      var columnSelectors = string.Join(", ", columnNames.Select(s => string.Format("[{0}]", s)).ToArray());
      selectCommand.CommandText = string.Format("SELECT {0} FROM [{1}]", columnSelectors, tableName);

      var toString = string.IsNullOrEmpty(dataFormat) ? "{0}" : "{0:" + dataFormat + "}";

      var importedDataEnumerator = dataValues.GetEnumerator();
      using (var dbReader = selectCommand.ExecuteReader())
      {
        while (dbReader.HasRows && dbReader.Read())
        {
          importedDataEnumerator.MoveNext();
          var importedData = importedDataEnumerator.Current;

          for (int column = 0; column < columnNames.Length; column++)
          {
            var rawDbValue = dbReader[column];
            var dbValue = string.Format(CultureInfo.InvariantCulture, toString, rawDbValue);
            var columnName = columnNames[column];

            // Apply padding on char(xxx) and nchar(xxx)
            var value = importedData[column];
            var match = FixedCharType.Match(columnName);
            if (match.Success)
            {
              var size = int.Parse(match.Groups["size"].Value);
              value = value.PadRight(size);
            }

            StringAssert.Contains(value, dbValue, string.Format("Row {0} was not imported correctly", column));
          }
        }
      }
    }

    protected void ImportRows(string tableName, string[] columnNames, IEnumerable<string[]> dataValues)
    {
      using (var importer = new Importer(SqlConnection, tableName, columnNames))
      {
        importer.Import(dataValues);
      }
    }
  }
}
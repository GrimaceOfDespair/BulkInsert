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

using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Grimace.BulkInsert.Test
{
  [TestFixture]
  public class TypeTests : DatabaseTests
  {
    public static Regex ColumnName = new Regex(@"\W");

    [TestCase("Char(255)")]
    [TestCase("VarChar(255)")]
    [TestCase("NChar(255)")]
    [TestCase("NVarChar(255)")]
    [Test]
    public void ImportText(string columntype)
    {
      VerifyRows(columntype, "abcdefghijklmnopqrstuvwxyz");
    }

    [TestCase("TinyInt", "0")]
    [TestCase("TinyInt", "255")]
    [TestCase("SmallInt", "-32768")]
    [TestCase("SmallInt", "32767")]
    [TestCase("Int", "2147483647")]
    [TestCase("Int", "-2147483647")]
    [TestCase("BigInt", "-9223372036854775808")]
    [TestCase("BigInt", "9223372036854775807")]
    [Test]
    public void ImportInt(string columntype, string value)
    {
      VerifyRows(columntype, value);
    }

    [TestCase("Time", "00:00:00.0000000")]
    [TestCase("Time", "23:59:59.9999999")]
    [Test]
    public void ImportTime(string columntype, string value)
    {
      VerifyRows(columntype, value, @"hh\:mm\:ss\.fffffff");
    }

    [TestCase("Date", "0001-01-01")]
    [TestCase("Date", "9999-12-31")]
    [TestCase("SmallDateTime", "1900-01-01")]
    [TestCase("SmallDateTime", "2079-06-06")]
    [TestCase("DateTime", "1753-01-01")]
    [TestCase("DateTime", "9999-12-31")]
    [Test]
    public void ImportDateTime(string columntype, string value)
    {
      VerifyRows(columntype, value, "yyyy-MM-dd");
    }

    [TestCase("DateTime2", "0001-01-01 00:00:00.0000000")]
    [TestCase("DateTime2", "9999-12-31 23:59:59.9999999")]
    [Test]
    public void ImportDateTime2(string columntype, string value)
    {
      VerifyRows(columntype, value, "yyyy-MM-dd HH:mm:ss.fffffff");
    }

    [TestCase("DateTimeOffset", "0001-01-01 00:00:00.0000000 +00:00")]
    [TestCase("DateTimeOffset", "0001-01-01 11:00:00.0000000 -11:00")]
    [TestCase("DateTimeOffset", "9999-12-31 23:59:59.9999999 +00:00")]
    [TestCase("DateTimeOffset", "9999-12-31 12:59:59.9999999 +11:00")]
    [Test]
    public void ImportDateTimeOffset(string columntype, string value)
    {
      VerifyRows(columntype, value, "yyyy-MM-dd HH:mm:ss.fffffff zzz");
    }

    public void ImportDate(string columntype, string value)
    {
      VerifyRows(columntype, value);
    }

    public void VerifyRows(string column, string dataValue, string dataFormat = null)
    {
      var tableName = "Import_" + column;
      var columnNames = new[] {column};
      var dataValues = new[] { new[] { dataValue } };

      CreateTable(tableName, column);
      ImportRows(tableName, columnNames, dataValues);
      VerifyRows(tableName, columnNames, dataValues, dataFormat);
    }

  }
}
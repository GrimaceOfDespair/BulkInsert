using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using log4net;

namespace Grimace.BulkInsert.Test
{
  [TestFixture]
  public class ImportTests : DatabaseTests
  {
    // Initialize log4net (necessary for the assembly attribute to kick in)
    public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportVarChar(int rowCount)
    {
      ImportAndVerify(rowCount, "Text", "NVarChar(4000)");
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportInt(int rowCount)
    {
      ImportAndVerify(rowCount, "Number", "Int");
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportTextAndNumber(int rowCount)
    {
      ImportAndVerify(rowCount, "TextAndNumber", "Text", "Int");
    }

    private void ImportAndVerify(int rowCount, string testName, params string[] columnNames)
    {
      var tableName = "Import_" + testName;

      Func<IEnumerable<string[]>> getDataValues = () =>
        GetDataValues(string.Format(@"App_Data\{0}.txt", testName), rowCount);

      CreateTable(tableName, columnNames);
      ImportRows(tableName, columnNames, getDataValues());
      VerifyRows(tableName, columnNames, getDataValues());
    }

    private static IEnumerable<string[]> GetDataValues(string path, int rowCount)
    {
      return File
        .ReadLines(path)
        .Take(rowCount)
        .Select(line => line.Split('|'));
    }

    private void ClearRows(SqlConnection sqlConnection, string tableName)
    {
      var truncCommand = sqlConnection.CreateCommand();
      truncCommand.CommandText = string.Format("TRUNCATE TABLE [{0}]", tableName);
      truncCommand.ExecuteNonQuery();
    }

  }
}

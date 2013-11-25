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
    [TestCase(1, "VarChar")]
    [TestCase(10, "VarChar")]
    [TestCase(int.MaxValue, "VarChar")]
    [TestCase(1, "NVarChar")]
    [TestCase(10, "NVarChar")]
    [TestCase(int.MaxValue, "NVarChar")]
    public void ImportVarChar(int rowCount, string field)
    {
      ImportAndVerify(rowCount, "Import", field);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportNumber(int rowCount)
    {
      ImportAndVerify(rowCount, "Import", "Number");
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportTextAndNumber(int rowCount)
    {
      ImportAndVerify(rowCount, "Import", "Text", "Number");
    }

    private void ImportAndVerify(int rowCount, string tableName, params string[] columnNames)
    {
      var dataFile = string.Join("And", columnNames);
      Func<IEnumerable<string[]>> getDataValues = () =>
        GetDataValues(string.Format(@"App_Data\{0}.txt", dataFile), rowCount);

      ClearRows(SqlConnection, tableName);
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

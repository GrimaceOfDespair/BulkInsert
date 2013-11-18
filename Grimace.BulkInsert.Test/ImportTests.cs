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
  public class ImportTests
  {
    // Initialize log4net (necessary for the assembly attribute to kick in)
    public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public SqlConnection SqlConnection;

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

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportText(int rowCount)
    {
      ImportAndVerify(rowCount, "Import", "Text");
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
      ClearTable(SqlConnection, tableName);

      var dataFile = string.Join("And", columnNames);
      Func<IEnumerable<string[]>> getDataValues = () =>
        GetDataValues(string.Format(@"App_Data\{0}.txt", dataFile), rowCount);

      using (var importer = new Importer(SqlConnection, tableName, columnNames))
      {
        importer.Import(getDataValues());
      }

      VerifyRows(getDataValues(), tableName, columnNames);
    }

    private void VerifyRows(IEnumerable<string[]> dataValues, string tableName, string[] columnNames)
    {
      var selectCommand = SqlConnection.CreateCommand();
      var columnSelectors = string.Join(", ", columnNames.Select(s => string.Format("[{0}]", s)).ToArray());
      selectCommand.CommandText = string.Format("SELECT {0} FROM [{1}]", columnSelectors, tableName);

      var importedDataEnumerator = dataValues.GetEnumerator();
      using (var dbReader = selectCommand.ExecuteReader())
      {
        while (dbReader.HasRows && dbReader.Read())
        {
          importedDataEnumerator.MoveNext();
          var importedData = importedDataEnumerator.Current;

          for (int column = 0; column < columnNames.Length; column++)
          {
            StringAssert.Contains(dbReader[column].ToString(), importedData[column], string.Format("Row {0} was not imported correctly", column));
          }
        }
      }
    }

    private static IEnumerable<string[]> GetDataValues(string path, int rowCount)
    {
      return File
        .ReadLines(path)
        .Take(rowCount)
        .Select(line => line.Split('|'));
    }

    private void ClearTable(SqlConnection sqlConnection, string tableName)
    {
      var truncCommand = sqlConnection.CreateCommand();
      truncCommand.CommandText = string.Format("TRUNCATE TABLE [{0}]", tableName);
      truncCommand.ExecuteNonQuery();
    }

  }
}

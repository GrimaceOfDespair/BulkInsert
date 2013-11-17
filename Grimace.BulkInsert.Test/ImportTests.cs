using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Grimace.BulkInsert.Test
{
  [TestFixture]
  public class ImportTests
  {
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
      ClearTable(SqlConnection, "Text");

      using (var importer = new Importer(SqlConnection, "Text", "Text"))
      {
        importer.Import(File
                          .ReadLines(@"App_Data\Text.txt")
                          .Take(rowCount)
                          .Select(line => new[] {line}));
      }
    }

    private void ClearTable(SqlConnection sqlConnection, string tableName)
    {
      var truncCommand = sqlConnection.CreateCommand();
      truncCommand.CommandText = string.Format("TRUNCATE TABLE [{0}]", tableName);
      truncCommand.ExecuteNonQuery();
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(int.MaxValue)]
    public void ImportNumber(int rowCount)
    {
    }
  }
}

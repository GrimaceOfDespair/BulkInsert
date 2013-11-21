using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;

namespace Grimace.BulkInsert.Test
{
  public class DatabaseTests
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

  }
}
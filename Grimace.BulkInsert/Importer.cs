using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Grimace.BulkInsert.Exceptions;
using Grimace.BulkInsert.FormatFile;
using Grimace.BulkInsert.Resources;

namespace Grimace.BulkInsert
{
  public class Importer : IDisposable
  {
    public DbConnection DbConnection { get; private set; }
    public string TableName { get; private set; }
    public string[] ColumnNames { get; private set; }
    public string FormatFile { get; private set; }

    public Importer(DbConnection sqlConnection, string tableName, params string[] columnNames)
    {
      DbConnection = sqlConnection;
      TableName = tableName;
      ColumnNames = columnNames;

      var formatFileBuilder = new FormatFileBuilder(sqlConnection);
      var formatXml = formatFileBuilder.CreateXml(tableName, columnNames);
      var formatFile = Path.GetTempFileName();
      File.WriteAllText(formatFile, formatXml);
    }

    public void Import(IEnumerable<string[]> dataValues)
    {
      var pipeName = Guid.NewGuid().ToString();

      // Create a dummy pipe, because SqlServer will close the
      // first one before actually inserting any data, subsequently
      // connecting to the second.
      // See also: http://stackoverflow.com/questions/2197017/can-sql-server-bulk-insert-read-from-a-named-pipe-fifo
      using (new NamedPipe(pipeName))

      using (var namedPipe = new NamedPipe(pipeName, dataValues))
      {
        ExecuteInsert(namedPipe.Path);
      }
    }

    private void ExecuteInsert(string namedPipePath)
    {
      var insertCommand = DbConnection.CreateCommand();
      insertCommand.CommandText = string.Format("BULK INSERT [{0}] FROM '{1}' WITH (FORMATFILE='{2}')", TableName, namedPipePath, FormatFile);
      insertCommand.ExecuteNonQuery();
    }

    public void Dispose()
    {
      
    }
  }
}

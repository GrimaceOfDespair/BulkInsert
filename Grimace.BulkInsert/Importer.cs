﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Grimace.BulkInsert.Exceptions;
using Grimace.BulkInsert.Extensions;
using Grimace.BulkInsert.FormatFile;
using Grimace.BulkInsert.Resources;
using Grimace.BulkInsert.Schema;
using log4net;

namespace Grimace.BulkInsert
{
  public class Importer : IDisposable
  {
    public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public DbConnection DbConnection { get; private set; }
    public string TableName { get; private set; }
    public string FormatFile { get; private set; }
    protected DbColumn[] DbColumns { get; private set; }

    private readonly HashSet<string> _columnNames;

    public Importer(DbConnection sqlConnection, string tableName, params string[] columnNames)
    {
      DbConnection = sqlConnection;
      TableName = tableName;
      _columnNames = new HashSet<string>(columnNames);

      DbColumns = new SchemaProvider(sqlConnection)
        .GetColumns(tableName).ToArray();

      FormatFile = FormatFileBuilder.CreateFormatFile(DbColumns);

      if (Log.IsDebugEnabled)
      {
        Log.Debug(string.Format("Format file created: {0}", FormatFile.CreateLink()));
      }
    }

    public void Import(IEnumerable<string[]> dataValues)
    {
      var pipeName = Guid.NewGuid().ToString();
      var trimmedValues = dataValues.Select(Truncate);

      // Create a dummy pipe, because SqlServer will close the
      // first one before actually inserting any data, subsequently
      // connecting to the second.
      // See also: http://stackoverflow.com/questions/2197017/can-sql-server-bulk-insert-read-from-a-named-pipe-fifo
      using (new NamedPipe(pipeName))

      using (var namedPipe = new NamedPipe(pipeName, trimmedValues))
      {
        ExecuteInsert(namedPipe.Path);
      }
    }

    private string[] Truncate(string[] dataValueRow)
    {
      if (_columnNames.Count != dataValueRow.Length)
      {
        throw new ColumnMismatchException(
          string.Format("{0} fields found, while importer expected {1}",
                        dataValueRow.Length, DbColumns.Length));
      }

      var i = 0;

      return DbColumns
        .Select(dbColumn =>

                _columnNames.Contains(dbColumn.Name)
                  ? dataValueRow[i++].Truncate(dbColumn.MaxLength)
                  : "")

        .ToArray();
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

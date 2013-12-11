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
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using log4net;

namespace Grimace.BulkInsert
{
  public class NamedPipe : IDisposable
  {
    public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IEnumerable<string[]> _dataValueArrays;
    public string Server { get; private set; }
    public string Name { get; private set; }
    public NamedPipeServerStream Stream { get; private set; }

    public NamedPipe(string name) :
      this(name, ".", new[] { new string[0] })
    {
    }

    public NamedPipe(string name, IEnumerable<string[]> dataValues) :
      this(name, ".", dataValues)
    {
    }

    public NamedPipe(string name, string server, IEnumerable<string[]> dataValueArrays)
    {
      _dataValueArrays = dataValueArrays;

      Name = name;
      Server = server;
      Stream = new NamedPipeServerStream(name, PipeDirection.Out, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

      if (Log.IsDebugEnabled)
      {
        Log.DebugFormat("Listening on named pipe {0}", Path);
      }

      Stream.BeginWaitForConnection(OnConnection, this);
    }

    public string Path { get { return string.Format(@"\\{0}\pipe\{1}", Server, Name); } }

    public void OnConnection(IAsyncResult asyncResult)
    {
      Stream.EndWaitForConnection(asyncResult);

      if (Log.IsDebugEnabled)
      {
        Log.DebugFormat("Incoming connection on named pipe {0}", Path);
      }

      if (Stream.IsConnected == false)
      {
        throw new NamedPipeException("Stream unexpectedly disconnected");
      }

      try
      {
        WriteToStream();
      }
      catch( Exception e)
      {
          if (Log.IsErrorEnabled)
          {
            Log.Error("Error while writing stream", e);
          }
      }
      finally
      {
        Stream.Close();
      }

      if (Log.IsDebugEnabled)
      {
        Log.DebugFormat("done writing");
      }
    }

    private void WriteToStream()
    {
      using (var stringWriter = new StringWriter())
      {
        foreach (var dataValues in _dataValueArrays)
        {
          foreach (var dataValue in dataValues)
          {
            if (Log.IsDebugEnabled)
            {
              Log.DebugFormat("Writing field: {0}", dataValue);
            }

            stringWriter.Write(dataValue + "\0");
          }

          if (Log.IsDebugEnabled)
          {
            Log.DebugFormat("Writing row terminator");
          }

          stringWriter.Write("\0");

          //var buffer = Encoding.UTF8.GetBytes(stringWriter.ToString());
          //Stream.Write(buffer, 0, buffer.Length);
          //Stream.WaitForPipeDrain();

          //stringWriter.GetStringBuilder().Length = 0;
        }

        // Write the whole stream at once, because SQLServer requires so
        // http://stackoverflow.com/questions/2197017/can-sql-server-bulk-insert-read-from-a-named-pipe-fifo
        var buffer = Encoding.UTF8.GetBytes(stringWriter.ToString());
        Stream.Write(buffer, 0, buffer.Length);
      }

      Stream.Flush();
    }

    public void Dispose()
    {
    }
  }

  public class NamedPipeException : Exception
  {
    public NamedPipeException(string message) : base(message)
    {
    }

    public NamedPipeException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected NamedPipeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}

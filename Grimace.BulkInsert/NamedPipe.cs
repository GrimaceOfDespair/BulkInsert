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
      Stream = new NamedPipeServerStream(name, PipeDirection.InOut, 2, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

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
        }

        // Write the whole stream at once, because SQLServer requires so
        // http://stackoverflow.com/questions/2197017/can-sql-server-bulk-insert-read-from-a-named-pipe-fifo
        var buffer = Encoding.UTF8.GetBytes(stringWriter.ToString());
        Stream.Write(buffer, 0, buffer.Length);
      }
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

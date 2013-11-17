using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
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
      this(name, ".", new[] { new[] { "If you find this in the database, something went wrong" } })
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
      Stream = new NamedPipeServerStream(name, PipeDirection.Out, 2, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

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

      foreach (var dataValues in _dataValueArrays)
      {
        foreach (var dataValue in dataValues)
        {
          var dataBytes = Encoding.UTF8.GetBytes(dataValue);
          Stream.Write(dataBytes,0, dataBytes.Length);
          Stream.Write(new byte[] { 0 }, 0, 1);
        }
        Stream.Write(new byte[] { 0 }, 0, 1);
        Stream.Write(new byte[] { 0 }, 0, 1);
      }
    }

    public void Dispose()
    {
    }
  }
}

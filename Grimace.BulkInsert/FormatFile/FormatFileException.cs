using System;
using System.Runtime.Serialization;

namespace Grimace.BulkInsert.FormatFile
{
  public class FormatFileException : Exception
  {
    public FormatFileException(string message) : base(message)
    {
    }

    public FormatFileException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected FormatFileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
using System;
using System.Runtime.Serialization;

namespace Grimace.BulkInsert.Exceptions
{
  public class ColumnMismatchException : Exception
  {
    public ColumnMismatchException(string message) : base(message)
    {
    }

    public ColumnMismatchException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected ColumnMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}
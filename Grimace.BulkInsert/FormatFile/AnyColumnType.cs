using System;
using Grimace.BulkInsert.Extensions;

namespace Grimace.BulkInsert.FormatFile
{
  public partial class AnyColumnType
  {
    public override string ToString()
    {
      return this.SerializeToXml(GetType());
    }

    public bool Equals(AnyColumnType other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.sOURCEField, sOURCEField) && Equals(other.nAMEField, nAMEField) && Equals(other.nULLABLEField, nULLABLEField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      var other = obj as AnyColumnType;
      return other != null && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = (sOURCEField != null ? sOURCEField.GetHashCode() : 0);
        result = (result*397) ^ (nAMEField != null ? nAMEField.GetHashCode() : 0);
        result = (result*397) ^ nULLABLEField.GetHashCode();
        return result;
      }
    }
  }
}
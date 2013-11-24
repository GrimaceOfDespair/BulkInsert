namespace Grimace.BulkInsert.FormatFile
{
  public partial class NativeFixed
  {
    public bool Equals(NativeFixed other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.lENGTHField, lENGTHField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (NativeFixed)) return false;
      return Equals((NativeFixed) obj);
    }

    public override int GetHashCode()
    {
      return (lENGTHField != null ? lENGTHField.GetHashCode() : 0);
    }
  }
}
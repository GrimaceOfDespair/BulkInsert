namespace Grimace.BulkInsert.FormatFile
{
  public partial class NativePrefix
  {
    public bool Equals(NativePrefix other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.pREFIX_LENGTHField, pREFIX_LENGTHField) && Equals(other.mAX_LENGTHField, mAX_LENGTHField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (NativePrefix)) return false;
      return Equals((NativePrefix) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (pREFIX_LENGTHField.GetHashCode()*397) ^ (mAX_LENGTHField != null ? mAX_LENGTHField.GetHashCode() : 0);
      }
    }
  }
}
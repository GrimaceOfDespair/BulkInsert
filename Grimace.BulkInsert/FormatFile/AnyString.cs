namespace Grimace.BulkInsert.FormatFile
{
  public partial class AnyString
  {
    public bool Equals(AnyString other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return base.Equals(other) && Equals(other.lENGTHField, lENGTHField);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (base.GetHashCode()*397) ^ (lENGTHField != null ? lENGTHField.GetHashCode() : 0);
      }
    }
  }
}
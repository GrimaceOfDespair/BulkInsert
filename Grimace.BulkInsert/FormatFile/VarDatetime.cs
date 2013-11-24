namespace Grimace.BulkInsert.FormatFile
{
  public partial class VarDatetime
  {
    public bool Equals(VarDatetime other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return base.Equals(other) && Equals(other.sCALEField, sCALEField);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (base.GetHashCode()*397) ^ (sCALEField != null ? sCALEField.GetHashCode() : 0);
      }
    }
  }
}
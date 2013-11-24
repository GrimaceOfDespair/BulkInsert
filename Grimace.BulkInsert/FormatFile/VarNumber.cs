namespace Grimace.BulkInsert.FormatFile
{
  public partial class VarNumber
  {
    public bool Equals(VarNumber other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return base.Equals(other) && Equals(other.pRECISIONField, pRECISIONField) && Equals(other.sCALEField, sCALEField);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result*397) ^ (pRECISIONField != null ? pRECISIONField.GetHashCode() : 0);
        result = (result*397) ^ (sCALEField != null ? sCALEField.GetHashCode() : 0);
        return result;
      }
    }
  }
}
namespace Grimace.BulkInsert.FormatFile
{
  public partial class CharTerm
  {
    public bool Equals(CharTerm other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.tERMINATORField, tERMINATORField) && Equals(other.mAX_LENGTHField, mAX_LENGTHField) && Equals(other.cOLLATIONField, cOLLATIONField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (CharTerm)) return false;
      return Equals((CharTerm) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = (tERMINATORField != null ? tERMINATORField.GetHashCode() : 0);
        result = (result*397) ^ (mAX_LENGTHField != null ? mAX_LENGTHField.GetHashCode() : 0);
        result = (result*397) ^ (cOLLATIONField != null ? cOLLATIONField.GetHashCode() : 0);
        return result;
      }
    }
  }
}
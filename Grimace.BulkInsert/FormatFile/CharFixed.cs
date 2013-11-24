namespace Grimace.BulkInsert.FormatFile
{
  public partial class CharFixed
  {
    public bool Equals(CharFixed other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Equals(other.cOLLATIONField, cOLLATIONField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof (CharFixed)) return false;
      return Equals((CharFixed) obj);
    }

    public override int GetHashCode()
    {
      return (cOLLATIONField != null ? cOLLATIONField.GetHashCode() : 0);
    }
  }
}
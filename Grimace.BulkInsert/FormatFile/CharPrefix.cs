namespace Grimace.BulkInsert.FormatFile
{
  public partial class CharPrefix
  {
    public bool Equals(CharPrefix other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return base.Equals(other) && Equals(other.cOLLATIONField, cOLLATIONField);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return Equals(obj as CharPrefix);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (base.GetHashCode()*397) ^ (cOLLATIONField != null ? cOLLATIONField.GetHashCode() : 0);
      }
    }
  }
}
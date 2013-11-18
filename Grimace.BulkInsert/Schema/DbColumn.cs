namespace Grimace.BulkInsert.Schema
{
  public struct DbColumn
  {
    public string Id;
    public string Name;
    public string SqlType;
    public string Nullable;
    public int MaxLength;
    public string CollationName;
  }
}
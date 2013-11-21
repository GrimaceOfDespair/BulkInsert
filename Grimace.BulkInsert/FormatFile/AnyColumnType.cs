using Grimace.BulkInsert.Extensions;

namespace Grimace.BulkInsert.FormatFile
{
  public partial class AnyColumnType
  {
    public override string ToString()
    {
      return this.SerializeToXml(GetType());
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grimace.BulkInsert.Extensions;

namespace Grimace.BulkInsert.FormatFile
{
  public partial class AnyFieldType
  {
    public override string ToString()
    {
      return this.SerializeToXml(GetType());
    }
  }
}

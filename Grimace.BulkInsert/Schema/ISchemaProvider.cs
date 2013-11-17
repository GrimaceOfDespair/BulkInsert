using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimace.BulkInsert.Schema
{
  public interface ISchemaProvider
  {
    IEnumerable<DbColumn> GetColumns(string tableName);
  }
}

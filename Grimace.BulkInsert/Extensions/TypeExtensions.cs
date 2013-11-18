using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimace.BulkInsert.Extensions
{
  public static class TypeExtensions
  {
    public static T DbToClr<T>(this object obj, T defaultValue = default(T))
    {
      return
        obj == null || obj == DBNull.Value
          ? defaultValue
          : (T) obj;
    }
  }
}

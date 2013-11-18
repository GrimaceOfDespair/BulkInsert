using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimace.BulkInsert.Extensions
{
  public static class StringExtensions
  {
    public static string Truncate(this string value, int maxLength)
    {
      if (value == null) throw new ArgumentNullException("value");
      return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
  }
}

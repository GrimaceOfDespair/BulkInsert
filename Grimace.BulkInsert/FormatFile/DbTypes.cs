using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Grimace.BulkInsert.FormatFile
{
  public class DbTypes
  {
    public static bool IsText(SqlDbType columnType)
    {
      return columnType == SqlDbType.Char
             || columnType == SqlDbType.NChar
             || columnType == SqlDbType.NText
             || columnType == SqlDbType.NVarChar
             || columnType == SqlDbType.Text
             || columnType == SqlDbType.VarChar;
    }

    public static bool IsNumeric(SqlDbType columnType)
    {
      return columnType == SqlDbType.TinyInt
             || columnType == SqlDbType.BigInt
             || columnType == SqlDbType.Decimal
             || columnType == SqlDbType.Int
             || columnType == SqlDbType.SmallInt;
    }

    public static bool IsBoolean(SqlDbType columnType)
    {
      return columnType == SqlDbType.Bit;
    }

    public static SqlDbType GetSqlDbType(string columnType)
    {
      switch (columnType)
      {
        case "bigint":
          return SqlDbType.BigInt;
        case "bit":
          return SqlDbType.Bit;
        case "binary":
          return SqlDbType.Binary;
        case "char":
          return SqlDbType.Char;
        case "datetime":
          return SqlDbType.DateTime;
        case "decimal":
          return SqlDbType.Decimal;
        case "float":
          return SqlDbType.Float;
        case "image":
          return SqlDbType.Image;
        case "int":
          return SqlDbType.Int;
        case "money":
          return SqlDbType.Money;
        case "nchar":
          return SqlDbType.NChar;
        case "numeric":
          return SqlDbType.Decimal;
        case "nvarchar":
          return SqlDbType.NVarChar;
        case "real":
          return SqlDbType.Real;
        case "smalldatetime":
          return SqlDbType.SmallDateTime;
        case "smallint":
          return SqlDbType.SmallInt;
        case "smallmoney":
          return SqlDbType.SmallMoney;
        case "sql_variant":
          return SqlDbType.Variant;
        case "text":
          return SqlDbType.Text;
        case "timestamp":
          return SqlDbType.Timestamp;
        case "tinyint":
          return SqlDbType.TinyInt;
        case "uniqueidentifier":
          return SqlDbType.UniqueIdentifier;
        case "varbinary":
          return SqlDbType.VarBinary;
        case "varchar":
          return SqlDbType.VarChar;
        case "xml":
          return SqlDbType.Xml;
        default:
          return SqlDbType.Int;
      }
    }

    public static string GetColumnType(SqlDbType dbType)
    {
      switch (dbType)
      {
        case SqlDbType.BigInt:
          return "bigint";
        case SqlDbType.Bit:
          return "bit";
        case SqlDbType.Binary:
          return "binary";
        case SqlDbType.Char:
          return "char";
        case SqlDbType.Date:
          return "datetime";
        case SqlDbType.DateTime:
          return "datetime";
        case SqlDbType.DateTime2:
          return "datetime";
        case SqlDbType.DateTimeOffset:
          return "datetime";
        case SqlDbType.Decimal:
          return "numeric";
        case SqlDbType.Float:
          return "float";
        case SqlDbType.Image:
          return "image";
        case SqlDbType.Int:
          return "int";
        case SqlDbType.Money:
          return "money";
        case SqlDbType.NChar:
          return "nchar";
        case SqlDbType.NText:
          return "ntext";
        case SqlDbType.NVarChar:
          return "nvarchar";
        case SqlDbType.Real:
          return "real";
        case SqlDbType.SmallDateTime:
          return "smalldatetime";
        case SqlDbType.SmallInt:
          return "smallint";
        case SqlDbType.SmallMoney:
          return "smallmoney";
        case SqlDbType.Structured:
          return "";
        case SqlDbType.Text:
          return "text";
        case SqlDbType.Time:
          return "datetime";
        case SqlDbType.Timestamp:
          return "bigint";
        case SqlDbType.TinyInt:
          return "tinyint";
        case SqlDbType.Udt:
          return "";
        case SqlDbType.UniqueIdentifier:
          return "uniqueidentifier";
        case SqlDbType.VarBinary:
          return "varbinary";
        case SqlDbType.VarChar:
          return "varchar";
        case SqlDbType.Variant:
          return "";
        case SqlDbType.Xml:
          return "xml";
        default:
          return "";
      }
    }

    public static int GetMaxLength(string columnType)
    {
      switch (columnType)
      {
        case "bigint":
          return 21; // -9223372036854775808
        case "bit":
          return 1;
        case "binary":
          return int.MaxValue;
        case "char":
          return int.MaxValue;
        case "datetime":
          return 35; // 2007-05-08 12:35:29.1234567 +12:15
        case "decimal":
          return 41; // -9999999999999999999999999999999999999.9
        case "float":
          return 41; // -9999999999999999999999999999999999999.9
        case "image":
          return int.MaxValue;
        case "int":
          return 12; // -2147483648
        case "money":
          return 22; // -922337203685477580.8
        case "nchar":
          return int.MaxValue;
        case "numeric":
          return 41; // -9999999999999999999999999999999999999.9
        case "nvarchar":
          return int.MaxValue;
        case "real":
          return 41; // -9999999999999999999999999999999999999.9
        case "smalldatetime":
          return 35; // 2007-05-08 12:35:29.1234567 +12:15
        case "smallint":
          return 6; // -32768
        case "smallmoney":
          return 22; // -922337203685477580.8
        case "sql_variant":
          return int.MaxValue;
        case "text":
          return int.MaxValue;
        case "timestamp":
          return 35; // 2007-05-08 12:35:29.1234567 +12:15
        case "tinyint":
          return 3; // -32768
        case "uniqueidentifier":
          return 37; // xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        case "varbinary":
          return int.MaxValue;
        case "varchar":
          return int.MaxValue;
        case "xml":
          return int.MaxValue;
        default:
          return int.MaxValue;
      }
    }

    public static int GetCharWidth(string columnType)
    {
      switch (columnType)
      {
        default:
          return 1;
        case "nvarchar":
        case "nchar":
          return 2;
      }
    }
  }
}
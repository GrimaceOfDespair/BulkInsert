using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Grimace.BulkInsert.Extensions;
using Grimace.BulkInsert.FormatFile;
using Grimace.BulkInsert.Schema;
using NUnit.Framework;
using log4net;

namespace Grimace.BulkInsert.Test
{
  [TestFixture]
  public class FormatFileTests : DatabaseTests
  {
    // Initialize log4net (necessary for the assembly attribute to kick in)
    public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    [Test]
    public void GeneratedFileMatchesBcpOutput()
    {
      var customFormat = GenerateFormatFileWithFormatFileBuilder();

      bcpFormatType bcpFormat;
      var bcpFormatFile = GenerateFormatFileWithBcp();
      using (var bcpFormatStream = new FileStream(bcpFormatFile, FileMode.Open))
      {
        bcpFormat = (bcpFormatType) new XmlSerializer(typeof(bcpFormatType)).Deserialize(bcpFormatStream);
      }

      Log.InfoFormat("Format builder: {0}", customFormat.SerializeToXml().CreateLinkFromContent(".xml"));
      Log.InfoFormat("Bcp output: {0}", bcpFormat.SerializeToXml().CreateLinkFromContent(".xml"));

      CollectionAssert.AreEqual(bcpFormat.RECORD, customFormat.RECORD);
      CollectionAssert.AreEqual(bcpFormat.ROW, customFormat.ROW);
    }

    private string GenerateFormatFileWithBcp()
    {
      var bcpExecutable = Path.GetFullPath(@"..\..\..\Resources\bin\bcp.exe");
      var formatFile = ".xml".GetTempFilePath();
      var bcpArguments = string.Format("IMPORT format nul -f \"{0}\" -c -t \\0 -r \\0\\0 -x -T -d \"{1}\" -S \"{2}\"",
        formatFile, SqlConnection.Database, SqlConnection.DataSource);

      var bcpProcess = new Process
                         {
                           StartInfo = new ProcessStartInfo
                                         {
                                           UseShellExecute = false,
                                           RedirectStandardOutput = true,
                                           RedirectStandardError = true,
                                           CreateNoWindow = true,
                                           FileName = bcpExecutable,
                                           Arguments = bcpArguments,
                                         },
                         };
      bcpProcess.Start();
      bcpProcess.WaitForExit();

      var output = bcpProcess.StandardOutput.ReadToEnd();
      if (string.IsNullOrEmpty(output) == false) Log.InfoFormat("BCP: {0}", output);

      var error = bcpProcess.StandardError.ReadToEnd();
      if (string.IsNullOrEmpty(error) == false) Log.ErrorFormat("BCP Error: {0}", error);

      Assert.AreEqual(bcpProcess.ExitCode, 0, "BCP could not be executed. Unexpected exit code.");

      return formatFile;
    }

    private bcpFormatType GenerateFormatFileWithFormatFileBuilder()
    {
      var schemaProvider = new SchemaProvider(SqlConnection);
      var dbColumns = schemaProvider.GetColumns("Import");
      return FormatFileBuilder.CreateFormatType(dbColumns);
    }
  }
}

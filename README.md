<img src="https://www.myget.org/Content/images/badges/successful.svg">

BulkInsert
==========

Use SQLServer Bulk Insert to import bulk data from .NET without files

Regular bulk import scenario's involve running bcp to analyze the database,
then supply the resulting schema data to either bcp or BULK INSERT and last
but not least provide the data in some text format.

Luckily SQLServer BULK INSERT can also handle pipe streams instead of files.
By hooking up a named pipe and throwing data at it without even touching disk
(except for the format file), it's possible to create an equivalent for
regular INSERTs which tend to be slower if used in bulk.

Example usage:

      // Get your data from anywhere, not necessarilly from a file
      var values = new[]
                    {
                      new[] {"value1", "value2"}, // Row 1
                      new[] {"value3", "value4"}  // Row 2
                    };
      
      // Bulk import data into MyTable in Column1 and Column2
      using (var importer = new Importer(SqlConnection, "MyTable", "Column1", "Column2"))
      {
        importer.Import(values);
      }

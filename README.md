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

@echo off

:: Create C# classes to serialize XML files that use bulkloadschema.xsd

setlocal enabledelayedexpansion

set VSVARSALL=%ProgramFiles(x86)%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat

if not exist "%VSVARSALL%" (
  echo I'm sorry. This batch file has been designed to run only with VS2010 installed. Feel free to patch ;)
) else (
  call "%VSVARSALL%"
  
  echo Generating class file
  xsd.exe bulkloadschema.xsd /classes /l:CS /n:Grimace.BulkInsert.FormatFile /o:..\Grimace.BulkInsert\FormatFile 
)

endlocal
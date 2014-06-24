SourceCombiner (Command line parameters)
==============

Note: Arguments can be ordered however one pleases and arguments are 
not case sensitive.

The syntax is as follows:
/output={FileName}         REM The path to save the resultant file to       
/SearchPattern={WidCard}   REM The pattern to use for directory iteration
/{true/false}              REM True to iterate all sub-dirs, false to iterate only top-level
/InputDir={Directory}      REM A directory to iterate for files
/InputFile={FileName}      REM A specific file to append

Here is an example to combine all .cs files in a certain directory.
Note the quotations in the path with spaces:

/output=Result.cs
/SearchPattern=*.cs
/true
"/InputDir=D:\Documents\Visual Studio 2013\Projects\RubiksCubeSolver"

To use in Visual Studio, go to Project/Properties/Debug and under start options,
type in the arguments (do not include the .exe name of course).

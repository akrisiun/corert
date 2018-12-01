
# cd E:\Beta\dot64\corert\src\ILCompiler.Compiler\tests\
dotnet build -r win10_x64 -c Debug

$old = $PWD
cd e:\Beta\dot64\corert\bin
# ..\..\..\bin\native\xunit.console.netcore.exe .\System.Collections.Tests.dll  

& ./xunit.console.netcore.exe .\ILCompiler.Compiler.Tests.dll 
# -trait 

cd $old

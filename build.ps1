
dotnet restore src\System.Private.CoreLib\src\System.Private.CoreLib.csproj
dotnet build   src\System.Private.CoreLib\src\System.Private.CoreLib.csproj --no-restore -o $PWD/bin

dotnet restore src\ILCompiler.Compiler\src\ILCompiler.Compiler.csproj
dotnet build   src\ILCompiler.Compiler\src\ILCompiler.Compiler.csproj  --no-restore -o $PWD/bin
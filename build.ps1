
dotnet restore src\System.Private.CoreLib\src\System.Private.CoreLib.csproj
dotnet build   src\System.Private.CoreLib\src\System.Private.CoreLib.csproj --no-restore -o $PWD/bin

dotnet restore src\ILCompiler.Compiler\src\ILCompiler.Compiler.csproj
dotnet build   src\ILCompiler.Compiler\src\ILCompiler.Compiler.csproj  --no-restore -o $PWD/bin

dotnet restore src\System.Private.Threading\src\System.Private.Threading.csproj
dotnet build   src\System.Private.Threading\src\System.Private.Threading.csproj       --no-restore -o $PWD/bin

dotnet restore src\System.Private.Interop\src\System.Private.Interop.Mono.csproj
dotnet build   src\System.Private.Interop\src\System.Private.Interop.Mono.csproj      --no-restore -o $PWD/bin

dotnet restore src\System.Private.Interop\src\System.Private.Interop.CoreCLR.csproj
dotnet build   src\System.Private.Interop\src\System.Private.Interop.CoreCLR.csproj   --no-restore -o $PWD/bin

dotnet restore src\System.Private.DeveloperExperience.Console\src\System.Private.DeveloperExperience.Console.cs...
dotnet build src\System.Private.DeveloperExperience.Console\src\System.Private.DeveloperExperience.Console.csproj
dotnet publish src\System.Private.DeveloperExperience.Console\src\System.Private.DeveloperExperience.Console.csproj `
  -o $PWD/bin  -r win-x64 -c Debug

# dotnet restore src\System.Private.TypeLoader\src\System.Private.TypeLoader.csproj
# dotnet build   src\System.Private.TypeLoader\src\System.Private.TypeLoader.csproj  -o $PWD/bin

dotnet run src\System.Private.DeveloperExperience.Console\src\System.Private.DeveloperExperience.Console.csproj
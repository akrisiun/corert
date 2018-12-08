

@REM # Tools\dotnetcli\dotnet.exe %1

set USER
mkdir %USERPROFILE%\.dotnet
mkdir %USERPROFILE%\.dotnet\x64
copy dotnet.cmd %USERPROFILE%\.dotnet\x64 /Y

@REM e:\Beta\dot64\mono08\external\corert\Tools\dotnetcli\dotnet.exe %*
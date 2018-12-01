@if not defined _echo @echo off
setlocal EnableDelayedExpansion

@REM # & "c:\Program Files\CMake\bin\cmake.exe" . -G "Visual Studio 15 2017 Win64"

set __ThisScriptShort=%0

if /i "%1" == "/?"    goto HelpVarCall
if /i "%1" == "-?"    goto HelpVarCall
if /i "%1" == "/h"    goto HelpVarCall
if /i "%1" == "-h"    goto HelpVarCall
if /i "%1" == "/help" goto HelpVarCall
if /i "%1" == "-help" goto HelpVarCall

goto :NormalVarCall

:HelpVarCall
call %~dp0buildscripts\buildvars-setup.cmd -help
exit /b 1

:NormalVarCall
call %~dp0buildscripts\buildvars-setup.cmd %*

IF NOT ERRORLEVEL 1 goto AfterVarSetup
echo Setting build variables failed.
exit /b %ERRORLEVEL%

:AfterVarSetup

echo Commencing CoreRT Repo build
echo.

echo & ./buildscripts/build-native.cmd X64
call %~dp0buildscripts\build-native.cmd %*

IF NOT ERRORLEVEL 1 goto AfterNativeBuild
echo Native component build failed. Refer !__NativeBuildLog! for details.
exit /b %ERRORLEVEL%

:AfterNativeBuild

echo & ./buildscripts/build-managed.cmd X64
call %~dp0buildscripts\build-managed.cmd %*

IF NOT ERRORLEVEL 1 goto AfterManagedBuild
echo Managed component build failed. Refer !__BuildLog! for details.
exit /b %ERRORLEVEL%

:AfterManagedBuild

call %~dp0buildscripts\build-tests.cmd %*

IF NOT ERRORLEVEL 1 goto AfterTests
echo Tests failed.
exit /b %ERRORLEVEL%

:AfterTests

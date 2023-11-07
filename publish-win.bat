@echo OFF

REM This batch file assumes the following:
REM - .NET Framework 4.8 SDK is installed and in PATH
REM - 7-zip commandline (7z.exe) is installed and in PATH
REM - Git for Windows is installed and in PATH
REM - The relevant commandline programs are already downloaded
REM   and put into their respective folders
REM
REM If any of these are not satisfied, the operation may fail
REM in an unpredictable way and result in an incomplete output.

REM Set the current directory as a variable
set BUILD_FOLDER=%~dp0

REM Set the current commit hash
for /f %%i in ('git log --pretty^=%%H -1') do set COMMIT=%%i

REM Restore Nuget packages for all builds
echo Restoring Nuget packages
dotnet restore

REM .NET Framework 4.8 Debug
echo Building .NET Framework 4.8 debug
dotnet publish MPF\MPF.csproj -f net48 -r win-x86 -c Debug --self-contained true --version-suffix %COMMIT%
dotnet publish MPF\MPF.csproj -f net48 -r win-x64 -c Debug --self-contained true --version-suffix %COMMIT%

REM .NET Framework 4.8 Release
echo Building .NET Framework 4.8 release
dotnet publish MPF\MPF.csproj -f net48 -r win-x86 -c Release --self-contained true --version-suffix %COMMIT% -p:DebugType=None -p:DebugSymbols=false
dotnet publish MPF\MPF.csproj -f net48 -r win-x64 -c Release --self-contained true --version-suffix %COMMIT% -p:DebugType=None -p:DebugSymbols=false

REM Create MPF Debug archives
cd %BUILD_FOLDER%\MPF\bin\Debug\net48\win-x86\publish\
7z a -tzip %BUILD_FOLDER%\MPF_win-x86_debug.zip *
cd %BUILD_FOLDER%\MPF\bin\Debug\net48\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\MPF_win-x64_debug.zip *

REM Create MPF Release archives
cd %BUILD_FOLDER%\MPF\bin\Release\net48\win-x86\publish\
7z a -tzip %BUILD_FOLDER%\MPF_win-x86_release.zip *
cd %BUILD_FOLDER%\MPF\bin\Release\net48\win-x64\publish\
7z a -tzip %BUILD_FOLDER%\MPF_win-x64_release.zip *

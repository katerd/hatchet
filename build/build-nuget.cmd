@echo off
if [%1]==[] goto noVersion

msbuild ..\src\Hatchet\Hatchet.csproj /p:Configuration=Release
if errorlevel 1 goto buildFailure

..\src\Hatchet\.nuget\nuget.exe pack ..\src\Hatchet\Hatchet.csproj -Version %1 -Properties Configuration=Release
if errorlevel 1 goto packageFailure

echo ================================================
echo.
echo Package created for version %1

goto :eof

:noVersion
echo Version number not specified.
goto :eof

:buildFailure
echo Failed to build Hatchet.csproj
goto :eof

:packageFailure
echo Failed to create .nupkg
goto :eof

@echo off
setlocal

:: Set default configuration to Debug
set CONFIGURATION=Debug

:: Allow overriding if an argument is provided
if not "%1"=="" set CONFIGURATION=%1

echo Restoring dependencies...
dotnet restore || exit /b

echo Building the project in %CONFIGURATION% mode...
dotnet build --configuration %CONFIGURATION% || exit /b

echo Build completed successfully.

pause
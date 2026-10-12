@echo off
cd /d "%~dp0"
dotnet run --project GradeLedger.Console\GradeLedger.Console.csproj
pause

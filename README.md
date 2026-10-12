# GradeLedger — Student Grade Management System

A C# student grade manager with two interfaces: a Windows Forms dashboard and a keyboard-driven console application. Both use the same core logic and saved records.

## Requirements
- Windows 10/11 for the desktop interface.
- .NET 8 SDK (the SDK, not just the runtime).
- The console and checks can also run on Linux/macOS.
- No third-party NuGet packages are required.

## Run
Open PowerShell in this folder (the folder containing this README).

```powershell
dotnet --version
dotnet run --project GradeLedger.Desktop/GradeLedger.Desktop.csproj
```

Alternatively, double-click `Run-Desktop.bat` on Windows. Run the console with:

```powershell
dotnet run --project GradeLedger.Console/GradeLedger.Console.csproj
```

Close one interface before using the other: each loads its own in-memory copy. Multiple simultaneous instances can overwrite each other's changes.

## Features
- Add a student with a unique name and a whole-number grade from 0 to 100.
- Display all records alphabetically with grade categories.
- Search exact names, ignoring letter case and surrounding spaces.
- Calculate class average, highest and lowest grades.
- Update grades only for existing students.
- Remove records with confirmation.
- Automatically save changes to JSON; export the whole class to CSV.
- Use mouse controls, text input, Tab navigation, Enter in search, or the console menu.
- Desktop shortcuts: Ctrl+N clears the editor, Ctrl+F focuses search, Ctrl+S saves.

Categories: A = 90–100, B = 80–89, C = 70–79, D = 60–69, F = 0–59. These are demonstration categories, not a claim about ALU's grading policy.

## Required concepts
| Requirement | Implementation |
| --- | --- |
| `Dictionary<string, int>` | Private student dictionary in `GradeLedger.Core/GradeBook.cs` |
| Functions and control flow | Add, Search, Update, Summary; console switch/loop |
| Average, highest, lowest | `Average()`, `Max()`, `Min()` on dictionary values |
| Error handling | Validated names/grades; caught expected exceptions in both interfaces |
| Enum and struct | `GradeCategory`, `StudentRecord`, `GradeSummary` |
| Onscreen .NET UI | Windows Forms `Dashboard` |
| Keyboard input | Console menu, desktop text fields and shortcuts |
| Custom features | JSON persistence, CSV export, grade updates and confirmed removal |

## Saved data and errors
Data is stored at `%LOCALAPPDATA%\GradeLedger\students.json` on Windows. Both interfaces use this same path. No real student data is shipped in the repository.

Blank names, duplicate names, decimal/text grades, out-of-range grades, missing students and empty statistics are handled with clear messages. A corrupt saved file is not silently overwritten: back it up, fix or move it, then restart. A saving failure is reported; the current changes remain in memory. Grade changes are written via a temporary file before replacing the saved file.

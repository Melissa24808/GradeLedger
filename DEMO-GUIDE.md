# Demonstration guide

Aim for about 4–6 minutes. Explain the code in your own words and show your actual running application. This guide is not a recorded video.

1. Introduce GradeLedger: it stores names and grades using a C# dictionary, with a Windows dashboard and console menu sharing one core.
2. Start the dashboard. Add Alice = 90, Bob = 60 and Claire = 75. Show the table, average 75.00, highest 90 and lowest 60. Start with an empty saved file or explain existing records.
3. Search `alice` to demonstrate case-insensitive matching. Click Show all. Select Bob, change his grade to 80, then click Update grade. The new average is 81.67 for these three students.
4. Show errors: duplicate Alice; blank name; `hello` and `12.5` grades; 101; search Unknown; update Unknown with a valid grade. Explain that updating an unknown name does not create a student.
5. Export a CSV and open it. Close and restart the dashboard to show saved records. Demonstrate Ctrl+N and Ctrl+F.
6. Close the dashboard, start the console and show its display, average and highest/lowest commands. Demonstrate an invalid menu choice.
7. Show `GradeBook.cs`: dictionary definition; validation; `Average`, `Max` and `Min`; enum and record structs. Show a `try/catch` in either interface and the console loop/switch.
8. Run the automated checks and show the actual output. Finish by showing the repository and how to run from its README.

## Simple explanations to prepare for questions
- Dictionary: each unique name is a key; its grade is the value.
- Case-insensitive comparer: Alice and ALICE identify the same student.
- Functions: each method handles one operation, reused by both interfaces.
- `TryParse`: safely attempts to turn text into a whole number.
- Exceptions: validation rejects an operation; the UI or console catches the error and tells the user what to fix.
- Average: sum of grades divided by the number of students, represented as a decimal-capable value.
- Max/Min: return the largest/smallest dictionary values. Empty collections are checked first.
- Enum: gives readable names to the five grade categories.
- Record structs: group the fields of a student or class summary into a small value type.
- JSON: saves records between runs. CSV: exports a table for a spreadsheet.
- Shared core: changing a rule in one place makes both interfaces consistent.

## Manual checklist
- [ ] Desktop builds and launches; layout fits the window.
- [ ] Add, display, search, average, highest/lowest work.
- [ ] Update rejects missing students; duplicate and invalid inputs give messages.
- [ ] Empty class has no invalid statistics calculation.
- [ ] Row selection, keyboard shortcuts, export and restart persistence work.
- [ ] Console menu and automated checks run.
- [ ] Repository and video links are accessible to the assessor.

using GradeLedger.Core;

var book = new GradeBook();
try { book.Load(GradeBook.DefaultFile); }
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or ArgumentException)
{ Console.WriteLine($"Saved records could not be loaded: {ex.Message}"); return; }

string? Ask(string label) { Console.Write(label); return Console.ReadLine(); }
void Save()
{
    try { book.Save(GradeBook.DefaultFile); }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
    { Console.WriteLine($"Records changed in memory, but saving failed: {ex.Message}"); }
}
while (true)
{
    Console.WriteLine("\nGRADELEDGER | STUDENT RECORDS\n1 Add student   2 Display all   3 Search\n4 Average       5 Highest/Lowest\n6 Update grade  7 Remove        8 Export CSV   0 Exit");
    string? choice = Ask("Choose: ");
    if (choice is null or "0") break;
    try
    {
        switch (choice)
        {
            case "1":
                string? name = Ask("Student name: ");
                if (name is null) return;
                string? grade = Ask("Grade (0–100): ");
                if (grade is null) return;
                book.Add(name, GradeBook.ParseGrade(grade)); Save(); Console.WriteLine("Student added."); break;
            case "2":
                if (book.Records.Count == 0) Console.WriteLine("No students yet.");
                foreach (var record in book.Records) Console.WriteLine($"{record.Name,-30} {record.Grade,3}/100   {record.Category}");
                break;
            case "3":
                string? query = Ask("Student name: "); if (query is null) return;
                var found = book.Search(query); Console.WriteLine($"{found.Name}: {found.Grade}/100 ({found.Category})"); break;
            case "4": Console.WriteLine($"Average: {book.Summary().Average:F2}/100"); break;
            case "5": var summary = book.Summary(); Console.WriteLine($"Highest: {summary.Highest} | Lowest: {summary.Lowest}"); break;
            case "6":
                string? update = Ask("Existing student name: "); if (update is null) return;
                string? newGrade = Ask("New grade: "); if (newGrade is null) return;
                book.Update(update, GradeBook.ParseGrade(newGrade)); Save(); Console.WriteLine("Grade updated."); break;
            case "7":
                string? remove = Ask("Student name to remove: "); if (remove is null) return;
                if (Ask("Type YES to confirm: ") == "YES") { book.Remove(remove); Save(); Console.WriteLine("Student removed."); }
                break;
            case "8":
                string? path = Ask("CSV filename/path: "); if (path is null) return;
                book.ExportCsv(path); Console.WriteLine("CSV exported."); break;
            default: Console.WriteLine("Choose one of the listed menu numbers."); break;
        }
    }
    catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException or InvalidOperationException or IOException or UnauthorizedAccessException or NotSupportedException)
    { Console.WriteLine($"Notice: {ex.Message}"); }
}

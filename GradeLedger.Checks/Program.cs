using GradeLedger.Core;

int passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAILED: " + name);
    Console.WriteLine("PASS: " + name); passed++;
}
void Reject<T>(Action action, string name) where T : Exception
{
    try { action(); } catch (T) { Check(true, name); return; }
    throw new Exception("FAILED: " + name + " did not reject input");
}
var book = new GradeBook();
Reject<InvalidOperationException>(() => book.Summary(), "empty statistics");
book.Add(" Alice ", 90); book.Add("Bob", 60); book.Add("Claire", 75);
Check(book.Search("ALICE").Grade == 90, "trimmed and case-insensitive names");
Check(book.Summary() == new GradeSummary(3, 75, 90, 60), "average, Max and Min");
Reject<ArgumentException>(() => book.Add("alice", 50), "duplicate name");
Reject<ArgumentException>(() => book.Add(" ", 50), "blank name");
Reject<ArgumentException>(() => GradeBook.ParseGrade("abc"), "non-numeric grade");
Reject<ArgumentException>(() => GradeBook.ParseGrade("12.5"), "fractional grade");
Reject<ArgumentOutOfRangeException>(() => book.Add("X", 101), "grade above 100");
Reject<ArgumentOutOfRangeException>(() => book.Update("Bob", -1), "negative grade");
Reject<KeyNotFoundException>(() => book.Search("Unknown"), "missing search");
Reject<KeyNotFoundException>(() => book.Update("Unknown", 85), "missing update");
book.Update("bob", 80); Check(book.Search("Bob").Grade == 80, "update");
Check(GradeBook.CategoryFor(90) == GradeCategory.A && GradeBook.CategoryFor(80) == GradeCategory.B && GradeBook.CategoryFor(70) == GradeCategory.C && GradeBook.CategoryFor(60) == GradeCategory.D && GradeBook.CategoryFor(59) == GradeCategory.F, "category boundaries");
book.Add("Zero", 0); book.Add("Perfect", 100); Check(book.Summary().Lowest == 0 && book.Summary().Highest == 100, "inclusive grade limits");
string folder = Path.Combine(Path.GetTempPath(), "GradeLedger-checks-" + Guid.NewGuid());
Directory.CreateDirectory(folder);
try
{
    string json = Path.Combine(folder, "records.json"); book.Save(json);
    var loaded = new GradeBook(); loaded.Load(json); Check(loaded.Records.SequenceEqual(book.Records), "JSON round trip");
    File.WriteAllText(json, "[{\"Name\":\"Bad\",\"Grade\":999}]");
    Reject<ArgumentOutOfRangeException>(() => loaded.Load(json), "invalid saved grade");
    Check(loaded.Records.SequenceEqual(book.Records), "failed load preserves current records");
    book.Add("A, \"Quoted\"", 40); book.Add("=formula", 42);
    string csv = Path.Combine(folder, "export.csv"); book.ExportCsv(csv);
    string contents = File.ReadAllText(csv);
    Check(contents.Contains("\"A, \"\"Quoted\"\"\",40,F") && contents.Contains("\"'=formula\",42,F"), "CSV escaping and formula protection");
    book.Remove("BOB"); Reject<KeyNotFoundException>(() => book.Search("Bob"), "remove");
}
finally { Directory.Delete(folder, true); }
Console.WriteLine($"All {passed} checks passed.");

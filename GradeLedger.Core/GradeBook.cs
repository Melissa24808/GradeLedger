using System.Globalization;
using System.Text;
using System.Text.Json;

namespace GradeLedger.Core;

public enum GradeCategory { A, B, C, D, F }
public readonly record struct StudentRecord(string Name, int Grade)
{
    public GradeCategory Category => GradeBook.CategoryFor(Grade);
}
public readonly record struct GradeSummary(int Count, double Average, int Highest, int Lowest);

public sealed class GradeBook
{
    // The dictionary is the single source of truth. Names ignore letter case.
    private readonly Dictionary<string, int> students = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<StudentRecord> Records => students.OrderBy(s => s.Key, StringComparer.OrdinalIgnoreCase)
        .Select(s => new StudentRecord(s.Key, s.Value)).ToList();
    public static string DefaultFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GradeLedger", "students.json");

    private static string ValidName(string? name)
    {
        string clean = (name ?? "").Trim();
        if (clean.Length == 0) throw new ArgumentException("Enter a student name.");
        if (clean.Length > 100 || clean.Any(char.IsControl)) throw new ArgumentException("Use a name of 1–100 characters without control characters.");
        return clean;
    }
    public static int ParseGrade(string? input)
    {
        if (!int.TryParse(input, out int grade)) throw new ArgumentException("The grade must be a whole number from 0 to 100.");
        ValidateGrade(grade);
        return grade;
    }
    private static void ValidateGrade(int grade)
    {
        if (grade < 0 || grade > 100) throw new ArgumentOutOfRangeException(nameof(grade), "The grade must be between 0 and 100.");
    }
    public void Add(string name, int grade)
    {
        name = ValidName(name);
        ValidateGrade(grade);
        if (!students.TryAdd(name, grade)) throw new ArgumentException("This student already exists. Use Update grade.");
    }
    public StudentRecord Search(string name)
    {
        name = ValidName(name);
        if (!students.TryGetValue(name, out int grade)) throw new KeyNotFoundException($"Student '{name}' was not found.");
        string storedName = students.Keys.First(k => StringComparer.OrdinalIgnoreCase.Equals(k, name));
        return new StudentRecord(storedName, grade);
    }
    public void Update(string name, int grade)
    {
        StudentRecord record = Search(name); // Reject grade updates for unknown students.
        ValidateGrade(grade);
        students[record.Name] = grade;
    }
    public void Remove(string name)
    {
        StudentRecord record = Search(name);
        students.Remove(record.Name);
    }
    public GradeSummary Summary()
    {
        if (students.Count == 0) throw new InvalidOperationException("Add a student before calculating statistics.");
        return new GradeSummary(students.Count, students.Values.Average(), students.Values.Max(), students.Values.Min());
    }
    public static GradeCategory CategoryFor(int grade) => grade switch
    {
        >= 90 => GradeCategory.A, >= 80 => GradeCategory.B, >= 70 => GradeCategory.C,
        >= 60 => GradeCategory.D, _ => GradeCategory.F
    };
    public void Save(string path)
    {
        string fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        string temporary = fullPath + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(Records, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, fullPath, true);
    }
    public void Load(string path)
    {
        if (!File.Exists(path)) return;
        List<StudentRecord> records = JsonSerializer.Deserialize<List<StudentRecord>>(File.ReadAllText(path))
            ?? throw new InvalidDataException("The saved file contains no student list.");
        // Validate the complete file before replacing current records.
        var validated = new GradeBook();
        foreach (StudentRecord record in records) validated.Add(record.Name, record.Grade);
        students.Clear();
        foreach (StudentRecord record in validated.Records) students.Add(record.Name, record.Grade);
    }
    public void ExportCsv(string path)
    {
        var csv = new StringBuilder("Name,Grade,Category\r\n");
        foreach (StudentRecord record in Records)
        {
            // Prefix spreadsheet formulas before quoting user-controlled names.
            string name = record.Name;
            if ("=+-@".Contains(name[0])) name = "'" + name;
            csv.Append('"').Append(name.Replace("\"", "\"\"")).Append("\",")
                .Append(record.Grade.ToString(CultureInfo.InvariantCulture)).Append(',').Append(record.Category).Append("\r\n");
        }
        File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));
    }
}

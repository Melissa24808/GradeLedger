using GradeLedger.Core;

namespace GradeLedger.Desktop;

public sealed class Dashboard : Form
{
    private readonly GradeBook book = new();
    private readonly TextBox name = new() { PlaceholderText = "Student full name", Dock = DockStyle.Fill };
    private readonly TextBox grade = new() { PlaceholderText = "Whole number: 0–100", Dock = DockStyle.Fill };
    private readonly TextBox query = new() { PlaceholderText = "Search exact student name", Dock = DockStyle.Fill };
    private readonly Label status = new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
    private readonly Label totals = new() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
    private readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false, RowHeadersVisible = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
        AutoGenerateColumns = true
    };
    private bool storageReady = true;
    private static readonly Color Teal = Color.FromArgb(17, 94, 89);

    public Dashboard()
    {
        Text = "GradeLedger — Student Grade Management";
        Size = new Size(1100, 730); MinimumSize = new Size(950, 650);
        StartPosition = FormStartPosition.CenterScreen; Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(241, 245, 249);
        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 195));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var rail = new Panel { Dock = DockStyle.Fill, BackColor = Teal, Padding = new Padding(18) };
        rail.Controls.Add(new Label { Text = "GRADE\nLEDGER\n\nStudent records\n& class insights\n\n────────────\n\nKeyboard shortcuts\nCtrl + N  New\nCtrl + F  Search\nCtrl + S  Save\n\nGrade categories\nA   90–100\nB   80–89\nC   70–79\nD   60–69\nF    0–59", Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI", 11), Padding = new Padding(0, 16, 0, 0) });
        shell.Controls.Add(rail, 0, 0);
        var main = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(22), RowCount = 5, ColumnCount = 1 };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 70)); main.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); main.RowStyles.Add(new RowStyle(SizeType.Absolute, 55)); main.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        main.Controls.Add(new Label { Text = "Class overview\nManage records. Understand progress.", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 17, FontStyle.Bold) }, 0, 0);
        totals.BackColor = Color.White; totals.Padding = new Padding(12); main.Controls.Add(totals, 0, 1);
        var middle = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 15, 0, 0) };
        middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); middle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 255));
        grid.ColumnHeadersDefaultCellStyle.BackColor = Teal; grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.EnableHeadersVisualStyles = false; grid.ColumnHeadersHeight = 36; grid.RowTemplate.Height = 34;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 253, 250);
        grid.SelectionChanged += (_, _) => { if (grid.CurrentRow?.DataBoundItem is StudentRecord r) { name.Text = r.Name; grade.Text = r.Grade.ToString(); } };
        middle.Controls.Add(grid, 0, 0);
        var editor = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 10, Padding = new Padding(16, 0, 0, 0) };
        for (int i = 0; i < 9; i++) editor.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 0 ? 44 : 39));
        editor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        editor.Controls.Add(new Label { Text = "Record editor", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Fill }, 0, 0);
        editor.Controls.Add(new Label { Text = "Student name", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 1);
        editor.Controls.Add(name, 0, 2); editor.Controls.Add(new Label { Text = "Grade / 100", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 3); editor.Controls.Add(grade, 0, 4);
        editor.Controls.Add(Button("Add student", () => Change(() => book.Add(name.Text, GradeBook.ParseGrade(grade.Text)), "Student added.")), 0, 5);
        editor.Controls.Add(Button("Update grade", () => Change(() => book.Update(name.Text, GradeBook.ParseGrade(grade.Text)), "Grade updated.")), 0, 6);
        editor.Controls.Add(Button("Remove student", RemoveStudent), 0, 7);
        editor.Controls.Add(Button("Clear fields", ClearFields), 0, 8);
        editor.Controls.Add(new Label { Text = "Select a row to edit.\nNames are unique and searches ignore case.\n\nRecords save automatically after changes.", Dock = DockStyle.Fill, ForeColor = Color.DimGray, Padding = new Padding(0, 10, 0, 0) }, 0, 9);
        middle.Controls.Add(editor, 1, 0); main.Controls.Add(middle, 0, 2);
        var searchRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, Padding = new Padding(0, 10, 0, 0) };
        searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) searchRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        searchRow.Controls.Add(query, 0, 0); searchRow.Controls.Add(Button("Search", SearchStudent), 1, 0);
        searchRow.Controls.Add(Button("Show all", () => { query.Clear(); RefreshView(); Notify("All records displayed."); }), 2, 0);
        searchRow.Controls.Add(Button("Export CSV", Export), 3, 0); main.Controls.Add(searchRow, 0, 3); main.Controls.Add(status, 0, 4);
        shell.Controls.Add(main, 1, 0); Controls.Add(shell);
        query.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { SearchStudent(); e.SuppressKeyPress = true; } };
        KeyPreview = true; KeyDown += (_, e) =>
        {
            if (!e.Control) return;
            if (e.KeyCode == Keys.N) ClearFields(); else if (e.KeyCode == Keys.F) query.Focus();
            else if (e.KeyCode == Keys.S) Safe(() => { if (!storageReady) throw new InvalidOperationException("Fix the unreadable saved file and restart before saving."); book.Save(GradeBook.DefaultFile); Notify("Records saved."); });
            else return;
            e.SuppressKeyPress = true;
        };
        Shown += (_, _) =>
        {
            try { book.Load(GradeBook.DefaultFile); RefreshView(); ClearFields(); Notify("Ready. Add your first student or select an existing record."); }
            catch (Exception ex) when (Expected(ex)) { storageReady = false; Notify($"Saved file could not be loaded: {ex.Message}. Back up/fix {GradeBook.DefaultFile} and restart.", true); }
        };
    }
    private static Button Button(string text, Action action)
    {
        var button = new Button { Text = text, Dock = DockStyle.Fill, BackColor = Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(2, 3, 2, 3) };
        button.FlatAppearance.BorderSize = 0; button.Click += (_, _) => action(); return button;
    }
    private static bool Expected(Exception ex) => ex is ArgumentException or KeyNotFoundException or InvalidOperationException or IOException or UnauthorizedAccessException or System.Text.Json.JsonException or NotSupportedException;
    private void Safe(Action action)
    {
        try { action(); } catch (Exception ex) when (Expected(ex)) { Notify(ex.Message, true); }
    }
    private void Change(Action action, string message) => Safe(() =>
    {
        if (!storageReady) throw new InvalidOperationException("Fix the unreadable saved file and restart before changing records.");
        action(); RefreshView();
        try { book.Save(GradeBook.DefaultFile); Notify(message + " Saved."); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { Notify(message + " Changed in memory, but saving failed: " + ex.Message, true); }
    });
    private void RefreshView()
    {
        grid.DataSource = book.Records.ToList();
        if (book.Records.Count == 0) totals.Text = "0 students     |     Average —     |     Highest —     |     Lowest —";
        else { var s = book.Summary(); totals.Text = $"{s.Count} students     |     Average {s.Average:F2}     |     Highest {s.Highest}     |     Lowest {s.Lowest}"; }
    }
    private void SearchStudent() => Safe(() =>
    {
        var record = book.Search(query.Text); grid.DataSource = new List<StudentRecord> { record };
        Notify($"Found {record.Name}: {record.Grade}/100, category {record.Category}. Statistics cover the whole class.");
    });
    private void ClearFields() { grid.ClearSelection(); name.Clear(); grade.Clear(); name.Focus(); }
    private void RemoveStudent()
    {
        if (MessageBox.Show(this, $"Remove '{name.Text}'?", "Confirm removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            Change(() => book.Remove(name.Text), "Student removed.");
    }
    private void Export() => Safe(() =>
    {
        using var dialog = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv", FileName = "class-grades.csv" };
        if (dialog.ShowDialog(this) == DialogResult.OK) { book.ExportCsv(dialog.FileName); Notify("All class records exported."); }
    });
    private void Notify(string message, bool error = false) { status.Text = message; status.ForeColor = error ? Color.Firebrick : Teal; }
}

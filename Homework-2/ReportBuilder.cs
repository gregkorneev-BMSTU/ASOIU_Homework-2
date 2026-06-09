using System.Text;

public class ReportBuilder
{
    private readonly DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();

    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    public ReportBuilder Title(string text)
    {
        _title = text;
        return this;
    }

    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    public string Build()
    {
        var (columns, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (_title.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"=== {_title} ===");
        }

        string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
        int columnCount = displayHeaders.Length;
        int[] widths = GetColumnWidths(columnCount);

        for (int i = 0; i < columnCount; i++)
        {
            sb.Append(FormatCell(displayHeaders[i], widths[i]));
        }

        sb.AppendLine();

        int totalWidth = 0;
        for (int i = 0; i < columnCount; i++)
        {
            totalWidth += widths[i] + 2;
        }

        sb.AppendLine(new string('-', totalWidth));

        foreach (string[] row in rows)
        {
            for (int i = 0; i < columnCount; i++)
            {
                string value = i < row.Length ? row[i] : "";
                sb.Append(FormatCell(value, widths[i]));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void Print()
    {
        Console.Write(Build());
    }

    private int[] GetColumnWidths(int columnCount)
    {
        var widths = new int[columnCount];

        for (int i = 0; i < columnCount; i++)
        {
            widths[i] = i < _widths.Length ? _widths[i] : 20;
        }

        return widths;
    }

    private static string FormatCell(string value, int width)
    {
        if (value.Length > width)
        {
            value = value[..width];
        }

        return value.PadRight(width) + "  ";
    }
}

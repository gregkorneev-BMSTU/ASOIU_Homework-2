using System.Text;

/// <summary>
/// Построитель отчётов с применением паттерна Fluent Interface.
/// </summary>
public class ReportBuilder
{
    private readonly DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = Array.Empty<string>();
    private int[] _widths = Array.Empty<int>();

    /// <summary>
    /// Создаёт построитель отчётов.
    /// </summary>
    /// <param name="db">Менеджер базы данных для выполнения SQL-запросов.</param>
    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    /// <summary>
    /// Задаёт SQL-запрос отчёта.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <returns>Текущий объект ReportBuilder.</returns>
    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    /// <summary>
    /// Задаёт заголовок отчёта.
    /// </summary>
    /// <param name="text">Текст заголовка.</param>
    /// <returns>Текущий объект ReportBuilder.</returns>
    public ReportBuilder Title(string text)
    {
        _title = text;
        return this;
    }

    /// <summary>
    /// Задаёт заголовки колонок отчёта.
    /// </summary>
    /// <param name="columns">Названия колонок.</param>
    /// <returns>Текущий объект ReportBuilder.</returns>
    public ReportBuilder Header(params string[] columns)
    {
        _headers = columns;
        return this;
    }

    /// <summary>
    /// Задаёт ширину колонок отчёта.
    /// </summary>
    /// <param name="widths">Ширины колонок в символах.</param>
    /// <returns>Текущий объект ReportBuilder.</returns>
    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    /// <summary>
    /// Формирует отчёт в виде строки.
    /// </summary>
    /// <returns>Готовый текст отчёта.</returns>
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

    /// <summary>
    /// Формирует отчёт и выводит его в консоль.
    /// </summary>
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

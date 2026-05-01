using Microsoft.Data.Sqlite;

/// <summary>
/// Инкапсулирует работу приложения с базой данных SQLite.
/// </summary>
public class DatabaseManager
{
    private readonly string _connectionString;

    /// <summary>
    /// Создаёт менеджер базы данных и таблицы, если они ещё не существуют.
    /// </summary>
    /// <param name="databasePath">Путь к файлу базы данных.</param>
    public DatabaseManager(string databasePath)
    {
        _connectionString = $"Data Source={databasePath};Foreign Keys=True";
        CreateTables();
    }

    /// <summary>
    /// Импортирует данные из CSV-файлов, если таблицы базы данных пустые.
    /// </summary>
    /// <param name="airlinesCsvPath">Путь к CSV-файлу авиакомпаний.</param>
    /// <param name="flightsCsvPath">Путь к CSV-файлу рейсов.</param>
    public void InitializeDatabase(string airlinesCsvPath, string flightsCsvPath)
    {
        if (GetTableCount("airline") == 0 && File.Exists(airlinesCsvPath))
        {
            ImportAirlinesFromCsv(airlinesCsvPath);
        }

        if (GetTableCount("flight") == 0 && File.Exists(flightsCsvPath))
        {
            ImportFlightsFromCsv(flightsCsvPath);
        }
    }

    /// <summary>
    /// Импортирует авиакомпании и рейсы из CSV-файлов.
    /// </summary>
    /// <param name="airlinesCsvPath">Путь к CSV-файлу авиакомпаний.</param>
    /// <param name="flightsCsvPath">Путь к CSV-файлу рейсов.</param>
    public void ImportFromCsv(string airlinesCsvPath, string flightsCsvPath)
    {
        ImportAirlinesFromCsv(airlinesCsvPath);
        ImportFlightsFromCsv(flightsCsvPath);
    }

    /// <summary>
    /// Возвращает список всех авиакомпаний.
    /// </summary>
    /// <returns>Список авиакомпаний.</returns>
    public List<Airline> GetAllAirlines()
    {
        var result = new List<Airline>();

        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT airline_id, airline_name
FROM airline
ORDER BY airline_id";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Airline(
                reader.GetInt32(0),
                reader.GetString(1)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает авиакомпанию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор авиакомпании.</param>
    /// <returns>Авиакомпания или null, если запись не найдена.</returns>
    public Airline? GetAirlineById(int id)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT airline_id, airline_name
FROM airline
WHERE airline_id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Airline(
            reader.GetInt32(0),
            reader.GetString(1));
    }

    /// <summary>
    /// Возвращает список всех рейсов.
    /// </summary>
    /// <returns>Список рейсов.</returns>
    public List<Flight> GetAllFlights()
    {
        var result = new List<Flight>();

        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT flight_id, airline_id, flight_name, distance_km
FROM flight
ORDER BY flight_id";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Flight(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return result;
    }

    /// <summary>
    /// Возвращает рейс по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор рейса.</param>
    /// <returns>Рейс или null, если запись не найдена.</returns>
    public Flight? GetFlightById(int id)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT flight_id, airline_id, flight_name, distance_km
FROM flight
WHERE flight_id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new Flight(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetInt32(3));
    }

    /// <summary>
    /// Возвращает список рейсов выбранной авиакомпании.
    /// </summary>
    /// <param name="airlineId">Идентификатор авиакомпании.</param>
    /// <returns>Список рейсов авиакомпании.</returns>
    public List<Flight> GetFlightsByAirline(int airlineId)
    {
        var result = new List<Flight>();

        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
SELECT flight_id, airline_id, flight_name, distance_km
FROM flight
WHERE airline_id = @airlineId
ORDER BY flight_name";
        command.Parameters.AddWithValue("@airlineId", airlineId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Flight(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }

        return result;
    }

    /// <summary>
    /// Добавляет рейс.
    /// </summary>
    /// <param name="flight">Добавляемый рейс.</param>
    public void AddFlight(Flight flight)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO flight (airline_id, flight_name, distance_km)
VALUES (@airlineId, @name, @distanceKm)";
        command.Parameters.AddWithValue("@airlineId", flight.AirlineId);
        command.Parameters.AddWithValue("@name", flight.Name);
        command.Parameters.AddWithValue("@distanceKm", flight.DistanceKm);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Обновляет рейс по идентификатору.
    /// </summary>
    /// <param name="flight">Рейс с новыми значениями.</param>
    public void UpdateFlight(Flight flight)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE flight
SET airline_id = @airlineId,
    flight_name = @name,
    distance_km = @distanceKm
WHERE flight_id = @id";
        command.Parameters.AddWithValue("@id", flight.Id);
        command.Parameters.AddWithValue("@airlineId", flight.AirlineId);
        command.Parameters.AddWithValue("@name", flight.Name);
        command.Parameters.AddWithValue("@distanceKm", flight.DistanceKm);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Удаляет рейс по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор рейса.</param>
    public void DeleteFlight(int id)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM flight WHERE flight_id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Выполняет SQL-запрос для отчётов и возвращает табличный результат.
    /// </summary>
    /// <param name="sql">SQL-запрос.</param>
    /// <returns>Имена колонок и строки результата.</returns>
    public (string[] Columns, List<string[]> Rows) ExecuteQuery(string sql)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = sql;

        using var reader = command.ExecuteReader();

        var columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            columns[i] = reader.GetName(i);
        }

        var rows = new List<string[]>();
        while (reader.Read())
        {
            var row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            }

            rows.Add(row);
        }

        return (columns, rows);
    }

    private void CreateTables()
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS airline(
    airline_id INTEGER PRIMARY KEY AUTOINCREMENT,
    airline_name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS flight(
    flight_id INTEGER PRIMARY KEY AUTOINCREMENT,
    airline_id INTEGER NOT NULL,
    flight_name TEXT NOT NULL,
    distance_km INTEGER NOT NULL,
    FOREIGN KEY (airline_id) REFERENCES airline(airline_id)
);";
        command.ExecuteNonQuery();
    }

    private int GetTableCount(string tableName)
    {
        using var connection = OpenConnection();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private void ImportAirlinesFromCsv(string path)
    {
        using var connection = OpenConnection();

        foreach (string[] fields in ReadCsvRows(path, 2))
        {
            if (!int.TryParse(fields[0], out int id))
            {
                continue;
            }

            var command = connection.CreateCommand();
            command.CommandText = @"
INSERT OR IGNORE INTO airline (airline_id, airline_name)
VALUES (@id, @name)";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", fields[1].Trim());
            command.ExecuteNonQuery();
        }
    }

    private void ImportFlightsFromCsv(string path)
    {
        using var connection = OpenConnection();

        foreach (string[] fields in ReadCsvRows(path, 4))
        {
            if (!int.TryParse(fields[0], out int id) ||
                !int.TryParse(fields[1], out int airlineId) ||
                !int.TryParse(fields[3], out int distanceKm))
            {
                continue;
            }

            var flight = new Flight(id, airlineId, fields[2].Trim(), distanceKm);
            var command = connection.CreateCommand();
            command.CommandText = @"
INSERT OR IGNORE INTO flight (flight_id, airline_id, flight_name, distance_km)
VALUES (@id, @airlineId, @name, @distanceKm)";
            command.Parameters.AddWithValue("@id", flight.Id);
            command.Parameters.AddWithValue("@airlineId", flight.AirlineId);
            command.Parameters.AddWithValue("@name", flight.Name);
            command.Parameters.AddWithValue("@distanceKm", flight.DistanceKm);
            command.ExecuteNonQuery();
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static List<string[]> ReadCsvRows(string path, int minFieldCount)
    {
        var rows = new List<string[]>();
        bool isFirstLine = true;

        foreach (string line in File.ReadLines(path))
        {
            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            string trimmedLine = line.Trim();
            if (trimmedLine.Length == 0)
            {
                continue;
            }

            string[] fields = trimmedLine.Split(';');
            if (fields.Length >= minFieldCount)
            {
                rows.Add(fields);
            }
        }

        return rows;
    }
}

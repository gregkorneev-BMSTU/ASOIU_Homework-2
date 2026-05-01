using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string dbPath = Path.Combine(AppContext.BaseDirectory, "airlines.db");
string airlinesCsvPath = Path.Combine(AppContext.BaseDirectory, "airlines.csv");
string flightsCsvPath = Path.Combine(AppContext.BaseDirectory, "flights.csv");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(airlinesCsvPath, flightsCsvPath);

string choice;
do
{
    Console.WriteLine("========================================");
    Console.WriteLine("      УПРАВЛЕНИЕ АВИАРЕЙСАМИ");
    Console.WriteLine("========================================");
    Console.WriteLine("1 - Показать все авиакомпании");
    Console.WriteLine("2 - Показать все рейсы");
    Console.WriteLine("3 - Добавить рейс");
    Console.WriteLine("4 - Редактировать рейс");
    Console.WriteLine("5 - Удалить рейс");
    Console.WriteLine("6 - Отчёты");
    Console.WriteLine("7 - Фильтр по авиакомпании");
    Console.WriteLine("0 - Выход");
    Console.Write("Ваш выбор: ");
    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                ShowAirlines(db);
                break;
            case "2":
                ShowFlights(db);
                break;
            case "3":
                AddFlight(db);
                break;
            case "4":
                EditFlight(db);
                break;
            case "5":
                DeleteFlight(db);
                break;
            case "6":
                ReportsMenu(db);
                break;
            case "7":
                FilterByAirline(db);
                break;
            case "0":
                break;
            default:
                Console.WriteLine("Неверный пункт меню.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }

    Console.WriteLine();
}
while (choice != "0");

static void ShowAirlines(DatabaseManager db)
{
    Console.WriteLine("--- Все авиакомпании ---");
    List<Airline> airlines = db.GetAllAirlines();

    foreach (Airline airline in airlines)
    {
        Console.WriteLine(airline);
    }

    Console.WriteLine($"Итого: {airlines.Count}");
}

static void ShowFlights(DatabaseManager db)
{
    Console.WriteLine("--- Все рейсы ---");
    List<Flight> flights = db.GetAllFlights();
    List<Airline> airlines = db.GetAllAirlines();

    foreach (Flight flight in flights)
    {
        string airlineName = GetAirlineName(airlines, flight.AirlineId);
        Console.WriteLine($"[{flight.Id}] {flight.Name}, авиакомпания: {airlineName}, дальность: {flight.DistanceKm} км");
    }

    Console.WriteLine($"Итого: {flights.Count}");
}

static void FilterByAirline(DatabaseManager db)
{
    Console.WriteLine("--- Фильтр по авиакомпании ---");
    int? airlineId = ReadAirlineId(db, "ID авиакомпании: ", null);
    if (airlineId == null)
    {
        return;
    }

    Airline? airline = db.GetAirlineById(airlineId.Value);
    List<Flight> flights = db.GetFlightsByAirline(airlineId.Value);

    if (flights.Count == 0)
    {
        Console.WriteLine("Для выбранной авиакомпании рейсы не найдены.");
        return;
    }

    Console.WriteLine($"Рейсы авиакомпании: {airline?.Name}");
    foreach (Flight flight in flights)
    {
        Console.WriteLine($"[{flight.Id}] {flight.Name}, дальность: {flight.DistanceKm} км");
    }

    Console.WriteLine($"Итого: {flights.Count}");
}

static void AddFlight(DatabaseManager db)
{
    Console.WriteLine("--- Добавление рейса ---");
    int? airlineId = ReadAirlineId(db, "ID авиакомпании: ", null);
    if (airlineId == null)
    {
        return;
    }

    string? name = ReadRequiredString("Название рейса: ", "Ошибка: название рейса не может быть пустым.");
    if (name == null)
    {
        return;
    }

    int? distanceKm = ReadInt("Дальность маршрута, км: ", "Ошибка: введите целое число.");
    if (distanceKm == null)
    {
        return;
    }

    var flight = new Flight(0, airlineId.Value, name, distanceKm.Value);
    db.AddFlight(flight);
    Console.WriteLine("Рейс добавлен.");
}

static void EditFlight(DatabaseManager db)
{
    Console.WriteLine("--- Редактирование рейса ---");
    Console.Write("Введите ID рейса: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Flight? flight = db.GetFlightById(id);
    if (flight == null)
    {
        Console.WriteLine($"Рейс с ID={id} не найден.");
        return;
    }

    Console.WriteLine($"Текущие данные: {flight}");
    Console.WriteLine("Нажмите Enter без ввода, чтобы оставить старое значение.");

    int? airlineId = ReadAirlineId(db, $"ID авиакомпании [{flight.AirlineId}]: ", flight.AirlineId);
    if (airlineId == null)
    {
        return;
    }

    string? name = ReadOptionalString($"Название рейса [{flight.Name}]: ", flight.Name);
    if (name == null)
    {
        return;
    }

    int? distanceKm = ReadOptionalInt(
        $"Дальность маршрута, км [{flight.DistanceKm}]: ",
        flight.DistanceKm,
        "Ошибка: дальность должна быть целым числом.");
    if (distanceKm == null)
    {
        return;
    }

    flight.AirlineId = airlineId.Value;
    flight.Name = name;
    flight.DistanceKm = distanceKm.Value;

    db.UpdateFlight(flight);
    Console.WriteLine("Рейс обновлён.");
}

static void DeleteFlight(DatabaseManager db)
{
    Console.WriteLine("--- Удаление рейса ---");
    Console.Write("Введите ID рейса: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Flight? flight = db.GetFlightById(id);
    if (flight == null)
    {
        Console.WriteLine($"Рейс с ID={id} не найден.");
        return;
    }

    Console.Write($"Удалить рейс \"{flight.Name}\"? (да/нет): ");
    string confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteFlight(id);
        Console.WriteLine("Рейс удалён.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine("1 - Полный список рейсов с авиакомпаниями");
        Console.WriteLine("2 - Количество рейсов по авиакомпаниям");
        Console.WriteLine("3 - Средняя дальность рейсов по авиакомпаниям");
        Console.WriteLine("0 - Назад");
        Console.Write("Ваш выбор: ");
        choice = Console.ReadLine()?.Trim() ?? "";

        switch (choice)
        {
            case "1":
                ReportFlightsWithAirlines(db);
                break;
            case "2":
                ReportFlightCountByAirline(db);
                break;
            case "3":
                ReportAverageDistanceByAirline(db);
                break;
            case "0":
                break;
            default:
                Console.WriteLine("Неверный пункт.");
                break;
        }

        Console.WriteLine();
    }
    while (choice != "0");
}

static void ReportFlightsWithAirlines(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"
SELECT f.flight_name, a.airline_name, f.distance_km
FROM flight f
JOIN airline a ON f.airline_id = a.airline_id
ORDER BY f.flight_name")
        .Title("Полный список рейсов с авиакомпаниями")
        .Header("Рейс", "Авиакомпания", "Дальность, км")
        .ColumnWidths(32, 24, 14)
        .Print();
}

static void ReportFlightCountByAirline(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"
SELECT a.airline_name, COUNT(*) AS flight_count
FROM flight f
JOIN airline a ON f.airline_id = a.airline_id
GROUP BY a.airline_name
ORDER BY a.airline_name")
        .Title("Количество рейсов по авиакомпаниям")
        .Header("Авиакомпания", "Кол-во рейсов")
        .ColumnWidths(24, 16)
        .Print();
}

static void ReportAverageDistanceByAirline(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query(@"
SELECT a.airline_name, ROUND(AVG(f.distance_km), 1) AS avg_distance_km
FROM flight f
JOIN airline a ON f.airline_id = a.airline_id
GROUP BY a.airline_name
ORDER BY avg_distance_km DESC")
        .Title("Средняя дальность рейсов по авиакомпаниям")
        .Header("Авиакомпания", "Средняя дальность, км")
        .ColumnWidths(24, 24)
        .Print();
}

static int? ReadInt(string prompt, string errorMessage)
{
    Console.Write(prompt);
    if (!int.TryParse(Console.ReadLine(), out int value))
    {
        Console.WriteLine(errorMessage);
        return null;
    }

    return value;
}

static int? ReadOptionalInt(string prompt, int currentValue, string errorMessage)
{
    Console.Write(prompt);
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length == 0)
    {
        return currentValue;
    }

    if (!int.TryParse(input, out int value))
    {
        Console.WriteLine(errorMessage);
        return null;
    }

    return value;
}

static string? ReadRequiredString(string prompt, string errorMessage)
{
    Console.Write(prompt);
    string value = Console.ReadLine()?.Trim() ?? "";
    if (value.Length == 0)
    {
        Console.WriteLine(errorMessage);
        return null;
    }

    return value;
}

static string? ReadOptionalString(string prompt, string currentValue)
{
    Console.Write(prompt);
    string value = Console.ReadLine()?.Trim() ?? "";
    return value.Length == 0 ? currentValue : value;
}

static int? ReadAirlineId(DatabaseManager db, string prompt, int? currentValue)
{
    Console.WriteLine("Доступные авиакомпании:");
    ShowAirlines(db);

    Console.Write(prompt);
    string input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length == 0)
    {
        if (currentValue == null)
        {
            Console.WriteLine("Ошибка: ID авиакомпании должен быть целым числом.");
        }

        return currentValue;
    }

    if (!int.TryParse(input, out int airlineId))
    {
        Console.WriteLine("Ошибка: ID авиакомпании должен быть целым числом.");
        return null;
    }

    if (db.GetAirlineById(airlineId) == null)
    {
        Console.WriteLine($"Ошибка: авиакомпания с ID={airlineId} не найдена.");
        return null;
    }

    return airlineId;
}

static string GetAirlineName(List<Airline> airlines, int airlineId)
{
    foreach (Airline airline in airlines)
    {
        if (airline.Id == airlineId)
        {
            return airline.Name;
        }
    }

    return $"#{airlineId}";
}

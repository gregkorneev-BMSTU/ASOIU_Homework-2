/// <summary>
/// Рейс основной таблицы.
/// </summary>
public class Flight
{
    private int _distanceKm;

    /// <summary>
    /// Идентификатор рейса.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор авиакомпании, к которой относится рейс.
    /// </summary>
    public int AirlineId { get; set; }

    /// <summary>
    /// Название рейса.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Дальность маршрута в километрах. Значение не может быть отрицательным.
    /// </summary>
    public int DistanceKm
    {
        get => _distanceKm;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Дальность маршрута не может быть отрицательной.");
            }

            _distanceKm = value;
        }
    }

    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="id">Идентификатор рейса.</param>
    /// <param name="airlineId">Идентификатор авиакомпании.</param>
    /// <param name="name">Название рейса.</param>
    /// <param name="distanceKm">Дальность маршрута в километрах.</param>
    public Flight(int id, int airlineId, string name, int distanceKm)
    {
        Id = id;
        AirlineId = airlineId;
        Name = name;
        DistanceKm = distanceKm;
    }

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public Flight() : this(0, 0, "", 0)
    {
    }

    /// <summary>
    /// Возвращает строковое представление рейса.
    /// </summary>
    /// <returns>Строка для вывода в консоль.</returns>
    public override string ToString()
        => $"[{Id}] {Name}, авиакомпания #{AirlineId}, дальность: {DistanceKm} км";
}

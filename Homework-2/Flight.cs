public class Flight
{
    private int _distanceKm;

    public int Id { get; set; }

    public int AirlineId { get; set; }

    public string Name { get; set; }

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

    public Flight(int id, int airlineId, string name, int distanceKm)
    {
        Id = id;
        AirlineId = airlineId;
        Name = name;
        DistanceKm = distanceKm;
    }

    public Flight() : this(0, 0, "", 0)
    {
    }

    public override string ToString()
        => $"[{Id}] {Name}, авиакомпания #{AirlineId}, дальность: {DistanceKm} км";
}

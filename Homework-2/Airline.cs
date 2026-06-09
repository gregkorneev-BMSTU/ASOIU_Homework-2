public class Airline
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Airline(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Airline() : this(0, "")
    {
    }

    public override string ToString()
        => $"[{Id}] {Name}";
}

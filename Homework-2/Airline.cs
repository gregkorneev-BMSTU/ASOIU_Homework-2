/// <summary>
/// Авиакомпания из справочника.
/// </summary>
public class Airline
{
    /// <summary>
    /// Идентификатор авиакомпании.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название авиакомпании.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="id">Идентификатор авиакомпании.</param>
    /// <param name="name">Название авиакомпании.</param>
    public Airline(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public Airline() : this(0, "")
    {
    }

    /// <summary>
    /// Возвращает строковое представление авиакомпании.
    /// </summary>
    /// <returns>Строка для вывода в консоль.</returns>
    public override string ToString()
        => $"[{Id}] {Name}";
}

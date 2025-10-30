namespace Models.Repos.A_Model;

public class EnumOption<T> where T : struct, Enum
{
    public T Value { get; set; }
    public string Text { get; set; } = "";
}
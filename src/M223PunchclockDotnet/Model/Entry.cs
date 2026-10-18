namespace M223PunchclockDotnet.Model;

public class Entry
{
    public int Id { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

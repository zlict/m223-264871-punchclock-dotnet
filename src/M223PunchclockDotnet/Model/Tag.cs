namespace M223PunchclockDotnet.Model;

public class Tag
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Entry> Entries { get; set; } = new List<Entry>();
}

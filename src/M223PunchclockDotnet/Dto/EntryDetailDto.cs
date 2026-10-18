namespace M223PunchclockDotnet.Dto;

public class EntryDetailDto
{
    public int Id { get; init; }
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }
    public CategoryDto? Category { get; init; }
    public List<TagDto> Tags { get; init; } = [];
}

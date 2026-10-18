using System.ComponentModel.DataAnnotations;

namespace M223PunchclockDotnet.Dto;

public class TagDto
{
    public int Id { get; internal set; }

    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;
}

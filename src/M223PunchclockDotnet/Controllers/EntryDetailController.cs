using M223PunchclockDotnet.Dto;
using M223PunchclockDotnet.Model;
using M223PunchclockDotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace M223PunchclockDotnet.Controllers;

[ApiController]
[Route("entry-details")]
public class EntryDetailController(EntryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EntryDetailDto>>> Get() =>
        Ok((await service.FindAll()).Select(ToDto).ToList());

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EntryDetailDto>> GetById(int id)
    {
        var entry = await service.FindById(id);
        return entry is null ? NotFound() : Ok(ToDto(entry));
    }

    private static EntryDetailDto ToDto(Entry entry) => new()
    {
        Id = entry.Id,
        CheckIn = entry.CheckIn,
        CheckOut = entry.CheckOut,
        Category = entry.Category is null ? null : new CategoryDto
        {
            Id = entry.Category.Id, Title = entry.Category.Title
        },
        Tags = entry.Tags.OrderBy(tag => tag.Id)
            .Select(tag => new TagDto { Id = tag.Id, Title = tag.Title }).ToList()
    };
}

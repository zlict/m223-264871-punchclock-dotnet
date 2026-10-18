using M223PunchclockDotnet.Dto;
using M223PunchclockDotnet.Model;
using M223PunchclockDotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace M223PunchclockDotnet.Controllers;

[ApiController]
[Route("categories")]
public class CategoryController(CategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> Get() =>
        Ok((await service.FindAll()).Select(ToDto).ToList());

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var entity = await service.FindById(id);
        return entity is null ? NotFound() : Ok(ToDto(entity));
    }

    [HttpPost]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto)
    {
        var entity = await service.Create(new Category { Title = dto.Title });
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(int id, CategoryDto dto)
    {
        var entity = await service.Update(id, dto.Title);
        return entity is null ? NotFound() : Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await service.FindById(id);
        if (entity is null) return NotFound();
        if (await service.IsReferenced(id))
            return Conflict(new ProblemDetails { Status = 409, Title = "Category is still referenced by entries." });
        await service.Delete(entity);
        return NoContent();
    }

    private static CategoryDto ToDto(Category entity) => new() { Id = entity.Id, Title = entity.Title };
}

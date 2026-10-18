using System.Net.Mime;
using M223PunchclockDotnet.Model;
using M223PunchclockDotnet.Dto;
using M223PunchclockDotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace M223PunchclockDotnet.Controllers
{
    [ApiController]
    [Route("entries")]
    public class EntryController : ControllerBase
    {
        private readonly EntryService _entryService;
        private readonly CategoryService _categoryService;
        private readonly TagService _tagService;

        public EntryController(EntryService entryService, CategoryService categoryService, TagService tagService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        [HttpGet]
        [ProducesResponseType<List<EntryDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var allEntries = await _entryService.FindAll();
            return Ok(allEntries.Select(ToDto).ToList());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntryDto>> GetById(int id)
        {
            var entry = await _entryService.FindById(id);
            return entry is null ? NotFound() : Ok(ToDto(entry));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<EntryDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EntryDto>> AddEntry(EntryDto dto)
        {
            var entry = await ToEntity(dto);
            if (entry is null) return NotFound();
            var newEntry = await _entryService.AddEntry(entry);
            return CreatedAtAction(nameof(GetById), new { id = newEntry.Id }, ToDto(newEntry));
        }

        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<EntryDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntryDto>> UpdateEntryAsync(int id, EntryDto dto)
        {
            if (await _entryService.FindById(id) is null) return NotFound();
            var entry = await ToEntity(dto);
            if (entry is null) return NotFound();
            var updatedEntry = await _entryService.UpdateEntryAsync(id, entry);
            return updatedEntry is null ? NotFound() : Ok(ToDto(updatedEntry));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEntryAsync(int id)
        {
            var deleted = await _entryService.DeleteEntryAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        private async Task<Entry?> ToEntity(EntryDto dto)
        {
            Category? category = null;
            if (dto.CategoryId is int categoryId)
            {
                category = await _categoryService.FindById(categoryId);
                if (category is null) return null;
            }

            var tags = new List<Tag>();
            foreach (var tagId in dto.TagIds.Distinct())
            {
                var tag = await _tagService.FindById(tagId);
                if (tag is null) return null;
                tags.Add(tag);
            }

            // Resolve all references before changing any tracked entry.
            return new Entry
            {
                CheckIn = dto.CheckIn!.Value,
                CheckOut = dto.CheckOut!.Value,
                Category = category,
                CategoryId = category?.Id,
                Tags = tags
            };
        }

        private static EntryDto ToDto(Entry entry) => new()
        {
            Id = entry.Id,
            CheckIn = entry.CheckIn,
            CheckOut = entry.CheckOut,
            CategoryId = entry.CategoryId,
            TagIds = entry.Tags.Select(tag => tag.Id).OrderBy(id => id).ToList()
        };

    }
}

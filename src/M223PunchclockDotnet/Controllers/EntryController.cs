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

        public EntryController(EntryService entryService)
        {
            _entryService = entryService;
        }

        [HttpGet]
        [ProducesResponseType<List<EntryDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var allEntries = await _entryService.FindAll();
            return Ok(allEntries.Select(ToDto).ToList());
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<EntryDto>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EntryDto>> AddEntry(EntryDto dto)
        {
            var newEntry = await _entryService.AddEntry(ToEntity(dto));
            return CreatedAtAction(nameof(Get), new { id = newEntry.Id }, ToDto(newEntry));
        }

        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<EntryDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EntryDto>> UpdateEntryAsync(int id, EntryDto dto)
        {
            var updatedEntry = await _entryService.UpdateEntryAsync(id, ToEntity(dto));
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

        private static Entry ToEntity(EntryDto dto) => new()
        {
            // ApiController validates the DTO before invoking the action.
            CheckIn = dto.CheckIn!.Value,
            CheckOut = dto.CheckOut!.Value
        };

        private static EntryDto ToDto(Entry entry) => new()
        {
            Id = entry.Id,
            CheckIn = entry.CheckIn,
            CheckOut = entry.CheckOut
        };

    }
}

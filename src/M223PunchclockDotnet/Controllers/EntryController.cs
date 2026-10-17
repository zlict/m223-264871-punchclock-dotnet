using System.Net.Mime;
using M223PunchclockDotnet.Model;
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
        [ProducesResponseType<Entry>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var allEntries = await _entryService.FindAll();
            return Ok(allEntries);
        }

        [HttpPost()]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<Entry>(StatusCodes.Status201Created)]
        public async Task<ActionResult<Entry>> AddEntry(Entry entry){
            var newElement = await _entryService.AddEntry(entry);

            return CreatedAtAction(nameof(Get), new{id = entry.Id}, entry);
        }


        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<Entry>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entry>> UpdateEntryAsync(int id, Entry entry)
        {
            // DateTime is a value type: Required alone cannot detect missing values.
            if (entry.CheckIn == default || entry.CheckOut == default)
            {
                return BadRequest("Check-in and check-out are required.");
            }

            if (entry.CheckIn.Kind != DateTimeKind.Utc || entry.CheckOut.Kind != DateTimeKind.Utc)
            {
                return BadRequest("Check-in and check-out must be UTC timestamps (Z).");
            }

            var updatedEntry = await _entryService.UpdateEntryAsync(id, entry);
            return updatedEntry is null ? NotFound() : Ok(updatedEntry);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEntryAsync(int id)
        {
            var deleted = await _entryService.DeleteEntryAsync(id);
            return deleted ? NoContent() : NotFound();
        }

    }
}

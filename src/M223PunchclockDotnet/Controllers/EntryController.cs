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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEntry(int id){
            var entryToDelete = await _entryService.FindById(id);
            if (entryToDelete == null)
            {
                return NotFound();
            }

            await _entryService.DeleteEntry(entryToDelete);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateEntry(int id, Entry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest();
            }

            var entryToUpdate = await _entryService.FindById(id);

            if (entryToUpdate == null)
            {
                return NotFound();
            }

            entryToUpdate.CheckIn = entry.CheckIn;
            entryToUpdate.CheckOut = entry.CheckOut;

            await _entryService.UpdateEntry(entryToUpdate);

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchEntry(int id, EntryPatchDto patch)
        {
            var entryToUpdate = await _entryService.FindById(id);

            if (entryToUpdate == null)
            {
                return NotFound();
            }

            if (patch.CheckIn.HasValue)
            {
                entryToUpdate.CheckIn = patch.CheckIn.Value;
            }

            if (patch.CheckOut.HasValue)
            {
                entryToUpdate.CheckOut = patch.CheckOut.Value;
            }

            await _entryService.UpdateEntry(entryToUpdate);
            return Ok(entryToUpdate);
        }
    }
}

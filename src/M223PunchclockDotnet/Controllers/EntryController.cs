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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEntry(int id)
        {
            var entry = await _entryService.FindById(id);
            if (entry == null)
            {
                return NotFound();
            }

            await _entryService.DeleteEntry(entry);
            return Ok();
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entry>> UpdateEntry(int id, Entry updatedEntry)  

        {
                var entry = await _entryService.FindById(id);
                if (entry == null)
                {
                    return NotFound();
                }
    
                entry.CheckIn = updatedEntry.CheckIn;
                entry.CheckOut = updatedEntry.CheckOut;
    
                await _entryService.UpdateEntry(entry);
    
                return Ok(entry);
            }


    }
}

using System.Globalization;
using M223PunchclockDotnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace M223PunchclockDotnet.Controllers;

[ApiController]
[Route("statistics")]
public class StatisticsController(EntryService entryService) : ControllerBase
{
    [HttpGet("time-summaries")]
    public async Task<ActionResult<Dictionary<string, string>>> GetTimeSummaries()
    {
        var result = new Dictionary<string, string>();
        foreach (var summary in await entryService.GetTimeSummariesAsync())
        {
            // TimeSpan.Hours alone wraps at 24; use total whole hours instead.
            var hours = summary.Duration.Ticks / TimeSpan.TicksPerHour;
            result.Add(summary.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                FormattableString.Invariant($"{hours:D2}:{summary.Duration.Minutes:D2}:{summary.Duration.Seconds:D2}"));
        }
        return Ok(result);
    }
}

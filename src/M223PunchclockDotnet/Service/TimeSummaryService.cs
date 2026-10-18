using M223PunchclockDotnet.Dto;
using M223PunchclockDotnet.Model;

namespace M223PunchclockDotnet.Service;

public class TimeSummaryService
{
    public List<TimeSummaryDto> CalculateSummaryPerDay(List<Entry> entries)
    {
        var summaries = new List<TimeSummaryDto>();
        foreach (var entry in entries)
        {
            var start = entry.CheckIn;
            var end = entry.CheckOut;
            if (start.Kind != DateTimeKind.Utc || end.Kind != DateTimeKind.Utc || end < start)
                throw new ArgumentException("Entries require UTC timestamps in chronological order.", nameof(entries));

            if (start == end)
                AddDuration(summaries, DateOnly.FromDateTime(start), TimeSpan.Zero);

            while (start < end)
            {
                // Avoid computing a day beyond DateTime.MaxValue for the last segment.
                var segmentEnd = start.Date == end.Date ? end : start.Date.AddDays(1);
                AddDuration(summaries, DateOnly.FromDateTime(start), segmentEnd - start);
                start = segmentEnd;
            }
        }
        return summaries.OrderBy(summary => summary.Date).ToList();
    }

    private static void AddDuration(List<TimeSummaryDto> summaries, DateOnly date, TimeSpan duration)
    {
        for (var i = 0; i < summaries.Count; i++)
        {
            if (summaries[i].Date == date)
            {
                summaries[i] = new TimeSummaryDto(date, summaries[i].Duration + duration);
                return;
            }
        }
        summaries.Add(new TimeSummaryDto(date, duration));
    }
}

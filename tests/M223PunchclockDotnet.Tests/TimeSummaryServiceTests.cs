using System.Globalization;
using M223PunchclockDotnet.Dto;
using M223PunchclockDotnet.Model;
using M223PunchclockDotnet.Service;
using NUnit.Framework;

namespace M223PunchclockDotnet.Tests;

[TestFixture]
public class TimeSummaryServiceTests
{
    [Test]
    public void ReturnsEmptyListForNoEntries()
    {
        AssertSummary([]);
    }

    [Test]
    public void SumsEntriesAndSortsDays()
    {
        AssertSummary([Entry("2024-08-21T08:00:00Z", "2024-08-21T15:45:00Z"),
            Entry("2024-08-20T13:00:00Z", "2024-08-20T17:30:00Z"),
            Entry("2024-08-20T08:00:00Z", "2024-08-20T12:00:00Z")],
            Summary("2024-08-20", TimeSpan.FromMinutes(510)), Summary("2024-08-21", TimeSpan.FromMinutes(465)));
    }

    [Test]
    public void SplitsMultipleDaysAndPreservesSeconds()
    {
        AssertSummary([Entry("2024-08-20T23:30:00Z", "2024-08-22T00:00:15Z")],
            Summary("2024-08-20", TimeSpan.FromMinutes(30)), Summary("2024-08-21", TimeSpan.FromHours(24)),
            Summary("2024-08-22", TimeSpan.FromSeconds(15)));
    }

    [Test]
    public void DoesNotAddDayWhenEntryEndsAtMidnight()
    {
        AssertSummary([Entry("2024-08-20T23:00:00Z", "2024-08-21T00:00:00Z")],
            Summary("2024-08-20", TimeSpan.FromHours(1)));
    }

    [Test]
    public void HandlesZeroDuration()
    {
        AssertSummary([Entry("2024-08-20T08:00:00Z", "2024-08-20T08:00:00Z")],
            Summary("2024-08-20", TimeSpan.Zero));
    }

    [Test]
    public void AddsOverlapsAndPreservesTotalsAbove24Hours()
    {
        AssertSummary([Entry("2024-08-20T00:00:00Z", "2024-08-20T20:00:00Z"),
            Entry("2024-08-20T00:00:00Z", "2024-08-20T20:00:00Z")], Summary("2024-08-20", TimeSpan.FromHours(40)));
    }

    [Test]
    public void SumsFractionalSecondsBeforeFormatting()
    {
        AssertSummary([Entry("2024-08-20T08:00:00Z", "2024-08-20T08:00:00.6Z"),
            Entry("2024-08-20T09:00:00Z", "2024-08-20T09:00:00.6Z")], Summary("2024-08-20", TimeSpan.FromSeconds(1.2)));
    }

    [Test]
    public void RejectsNegativeDuration()
    {
        var service = new TimeSummaryService();
        var entry = Entry("2024-08-20T09:00:00Z", "2024-08-20T08:00:00Z");

        Assert.Throws<ArgumentException>(() => service.CalculateSummaryPerDay([entry]));
    }

    [Test]
    public void RejectsNonUtcTimestamps()
    {
        var service = new TimeSummaryService();
        var entry = new Entry
        {
            CheckIn = new DateTime(2024, 8, 20),
            CheckOut = new DateTime(2024, 8, 21)
        };

        Assert.Throws<ArgumentException>(() => service.CalculateSummaryPerDay([entry]));
    }

    private static void AssertSummary(List<Entry> entries, params TimeSummaryDto[] expected)
    {
        var service = new TimeSummaryService();
        var actual = service.CalculateSummaryPerDay(entries);
        Assert.That(actual, Is.EqualTo(expected));
    }

    private static Entry Entry(string start, string end) => new()
    {
        CheckIn = DateTime.Parse(start, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
        CheckOut = DateTime.Parse(end, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)
    };

    private static TimeSummaryDto Summary(string date, TimeSpan duration) =>
        new(DateOnly.Parse(date, CultureInfo.InvariantCulture), duration);
}

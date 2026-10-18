using M223PunchclockDotnet.Model;
using Microsoft.EntityFrameworkCore;

namespace M223PunchclockDotnet.Service;

public class ExampleDataService(DatabaseContext databaseContext)
{
    public async Task CreateExampleDataAsync()
    {
        // Only seed a completely empty database; do not fill partial datasets.
        if (await databaseContext.Categories.AnyAsync()
            || await databaseContext.Tags.AnyAsync()
            || await databaseContext.Entries.AnyAsync())
        {
            return;
        }

        var project = new Category { Title = "Projektarbeit" };
        var lesson = new Category { Title = "Unterricht" };
        var administration = new Category { Title = "Administration" };
        databaseContext.Categories.AddRange(project, lesson, administration);

        var dotnet = new Tag { Title = ".NET" };
        var backend = new Tag { Title = "Backend" };
        var documentation = new Tag { Title = "Dokumentation" };
        databaseContext.Tags.AddRange(dotnet, backend, documentation);

        databaseContext.Entries.AddRange(
            new Entry
            {
                CheckIn = new DateTime(2026, 10, 12, 8, 0, 0, DateTimeKind.Utc),
                CheckOut = new DateTime(2026, 10, 12, 11, 30, 0, DateTimeKind.Utc),
                Category = project,
                Tags = [dotnet, backend]
            },
            new Entry
            {
                CheckIn = new DateTime(2026, 10, 12, 13, 0, 0, DateTimeKind.Utc),
                CheckOut = new DateTime(2026, 10, 12, 16, 0, 0, DateTimeKind.Utc),
                Category = lesson,
                Tags = [dotnet]
            },
            new Entry
            {
                CheckIn = new DateTime(2026, 10, 13, 8, 30, 0, DateTimeKind.Utc),
                CheckOut = new DateTime(2026, 10, 13, 9, 30, 0, DateTimeKind.Utc),
                Category = administration,
                Tags = [documentation]
            },
            new Entry
            {
                CheckIn = new DateTime(2026, 10, 13, 10, 0, 0, DateTimeKind.Utc),
                CheckOut = new DateTime(2026, 10, 13, 11, 0, 0, DateTimeKind.Utc)
            });

        // EF Core saves all entities and relationships together in one transaction.
        await databaseContext.SaveChangesAsync();
    }
}

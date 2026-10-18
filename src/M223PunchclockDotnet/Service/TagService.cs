using M223PunchclockDotnet.Model;
using Microsoft.EntityFrameworkCore;

namespace M223PunchclockDotnet.Service;

public class TagService(DatabaseContext databaseContext)
{
    public Task<List<Tag>> FindAll() => databaseContext.Tags.OrderBy(x => x.Id).ToListAsync();

    public Task<Tag?> FindById(int id) => databaseContext.Tags.SingleOrDefaultAsync(x => x.Id == id);

    public async Task<Tag> Create(Tag entity)
    {
        databaseContext.Tags.Add(entity);
        await databaseContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Tag?> Update(int id, string title)
    {
        var entity = await FindById(id);
        if (entity is null) return null;
        entity.Title = title;
        await databaseContext.SaveChangesAsync();
        return entity;
    }

    public Task<bool> IsReferenced(int id) =>
        databaseContext.Tags.AnyAsync(x => x.Id == id && x.Entries.Any());

    public async Task Delete(Tag entity)
    {
        databaseContext.Tags.Remove(entity);
        await databaseContext.SaveChangesAsync();
    }
}

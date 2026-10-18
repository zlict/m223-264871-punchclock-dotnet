using M223PunchclockDotnet.Model;
using Microsoft.EntityFrameworkCore;

namespace M223PunchclockDotnet.Service;

public class CategoryService(DatabaseContext databaseContext)
{
    public Task<List<Category>> FindAll() => databaseContext.Categories.OrderBy(x => x.Id).ToListAsync();

    public Task<Category?> FindById(int id) => databaseContext.Categories.SingleOrDefaultAsync(x => x.Id == id);

    public async Task<Category> Create(Category entity)
    {
        databaseContext.Categories.Add(entity);
        await databaseContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Category?> Update(int id, string title)
    {
        var entity = await FindById(id);
        if (entity is null) return null;
        entity.Title = title;
        await databaseContext.SaveChangesAsync();
        return entity;
    }

    public Task<bool> IsReferenced(int id) =>
        databaseContext.Categories.AnyAsync(x => x.Id == id && x.Entries.Any());

    public async Task Delete(Category entity)
    {
        databaseContext.Categories.Remove(entity);
        await databaseContext.SaveChangesAsync();
    }
}

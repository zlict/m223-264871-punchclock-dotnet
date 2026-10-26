using M223PunchclockDotnet.Model;
using Microsoft.EntityFrameworkCore;

namespace M223PunchclockDotnet.Service
{
    public class EntryService
    {
        private DatabaseContext _databaseContext; 

        public EntryService(DatabaseContext databaseContext){
            _databaseContext = databaseContext;
        }
        public Task<List<Entry>> FindAll()
        {
            return _databaseContext.Entries.ToListAsync();
        }

        public async Task<Entry> AddEntry(Entry entry)
        {
            _databaseContext.Entries.Add(entry);
            await _databaseContext.SaveChangesAsync();

            return entry;
        }
        public async Task<Entry> DeleteEntry(Entry entry)
        {
            _databaseContext.Entries.Remove(entry);
            await _databaseContext.SaveChangesAsync();

            return entry;
        }

        public async Task<Entry?> UpdateEntry(Entry entry)
        {
            _databaseContext.Entries.Update(entry);
            await _databaseContext.SaveChangesAsync();
            return entry;
        }

        public Task<Entry?> FindById(int id)
            => _databaseContext.Entries.FindAsync(id).AsTask();
    }
}

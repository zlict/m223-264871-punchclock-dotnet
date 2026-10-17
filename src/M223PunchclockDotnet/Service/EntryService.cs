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
        public async Task<Entry?> UpdateEntryAsync(int id, Entry entry)
        {
            var existingEntry = await _databaseContext.Entries.FindAsync(id);
            if (existingEntry is null)
            {
                return null;
            }

            // The route ID identifies the entity; never overwrite its primary key.
            existingEntry.CheckIn = entry.CheckIn;
            existingEntry.CheckOut = entry.CheckOut;
            await _databaseContext.SaveChangesAsync();
            return existingEntry;
        }

        public async Task<bool> DeleteEntryAsync(int id)
        {
            var entry = await _databaseContext.Entries.FindAsync(id);
            if (entry is null)
            {
                return false;
            }

            _databaseContext.Entries.Remove(entry);
            await _databaseContext.SaveChangesAsync();
            return true;
        }

    }
}

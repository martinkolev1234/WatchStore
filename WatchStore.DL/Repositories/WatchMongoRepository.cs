using MongoDB.Driver;
using WatchStore.BL.Services;
using WatchStore.Core.Models;

namespace WatchStore.DL.Repositories;

internal class WatchRepository : IWatchRepository
{
    private readonly IMongoCollection<Watch> _collection;

    public WatchRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Watch>("Watches");
        InitializeIndexes();
    }

    private void InitializeIndexes()
    {
        try
        {
            var indexKeys = Builders<Watch>.IndexKeys.Ascending(w => w.OwnerId);
            var indexOptions = new CreateIndexOptions { Unique = false };

            _collection.Indexes.CreateOne(new CreateIndexModel<Watch>(indexKeys, indexOptions));
        }
        catch (Exception)
        {
            Console.WriteLine("Warning: Failed to create index for Watches. Is MongoDB running?");
        }
    }

    public async Task AddWatchAsync(Watch watch)
        => await _collection.InsertOneAsync(watch);

    public async Task<IEnumerable<Watch>> GetAllWatchesAsync()
    {
        var watches = await _collection.Find(_ => true).ToListAsync();
        return watches;
    }

    public async Task<Watch?> GetWatchByIdAsync(Guid id)
        => await _collection.Find(w => w.Id == id).FirstOrDefaultAsync();

    public async Task DeleteWatchAsync(Guid id)
        => await _collection.DeleteOneAsync(w => w.Id == id);

    public async Task UpdateWatchAsync(Watch watch)
        => await _collection.ReplaceOneAsync(w => w.Id == watch.Id, watch);
}
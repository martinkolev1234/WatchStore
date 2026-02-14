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

    public void AddWatch(Watch watch)
        => _collection.InsertOne(watch);

    public IEnumerable<Watch> GetAllWatches()
        => _collection.Find(_ => true).ToList();

    public Watch? GetWatchById(Guid id)
        => _collection.Find(w => w.Id == id).FirstOrDefault();

    public void DeleteWatch(Guid id)
        => _collection.DeleteOne(w => w.Id == id);

    public void UpdateWatch(Watch watch)
        => _collection.ReplaceOne(w => w.Id == watch.Id, watch);
}
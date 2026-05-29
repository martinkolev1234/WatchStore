using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.StaticDataBase;

namespace WatchStore.DL.Repositories;

internal class WatchLocalRepository : IWatchRepository
{
    private static readonly object _lock = new();

    public Task AddWatchAsync(Watch watch)
    {
        lock (_lock)
        {
            StaticDb.Watches.Add(watch);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Watch>> GetAllWatchesAsync()
    {
        IEnumerable<Watch> result;
        lock (_lock)
        {
            result = StaticDb.Watches.ToList();
        }
        return Task.FromResult(result);
    }

    public Task<Watch?> GetWatchByIdAsync(Guid id)
    {
        Watch? result;
        lock (_lock)
        {
            result = StaticDb.Watches.FirstOrDefault(w => w.Id == id);
        }
        return Task.FromResult(result);
    }

    public Task UpdateWatchAsync(Watch watch)
    {
        lock (_lock)
        {
            var index = StaticDb.Watches.FindIndex(w => w.Id == watch.Id);

            if (index != -1)
            {
                StaticDb.Watches[index] = watch;
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteWatchAsync(Guid id)
    {
        lock (_lock)
        {
            StaticDb.Watches.RemoveAll(w => w.Id == id);
        }
        return Task.CompletedTask;
    }
}
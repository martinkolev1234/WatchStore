using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.StaticDataBase;

namespace WatchStore.DL.Repositories;

internal class WatchLocalRepository : IWatchRepository
{
    private static readonly object _lock = new();

    public void AddWatch(Watch watch)
    {
        lock (_lock)
        {
            StaticDb.Watches.Add(watch);
        }
    }

    public IEnumerable<Watch> GetAllWatches()
    {
        lock (_lock)
        {
            return StaticDb.Watches.ToList();
        }
    }

    public Watch? GetWatchById(Guid id)
    {
        lock (_lock)
        {
            return StaticDb.Watches.FirstOrDefault(w => w.Id == id);
        }
    }

    public void UpdateWatch(Watch watch)
    {
        lock (_lock)
        {
            var index = StaticDb.Watches.FindIndex(w => w.Id == watch.Id);

            if (index != -1)
            {
                StaticDb.Watches[index] = watch;
            }
        }
    }

    public void DeleteWatch(Guid id)
    {
        lock (_lock)
        {
            StaticDb.Watches.RemoveAll(w => w.Id == id);
        }
    }
}
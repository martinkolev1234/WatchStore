using WatchStore.Core.Models;

namespace WatchStore.DL.Cache;

public class WatchCache
{
    private readonly Dictionary<Guid, Watch> _cache = new();
    private readonly object _lock = new();

    public void Add(Watch watch)
    {
        lock (_lock) _cache[watch.Id] = watch;
    }

    public Watch? Find(Guid id)
    {
        lock (_lock) return _cache.TryGetValue(id, out var watch) ? watch : null;
    }

    public IReadOnlyCollection<Watch> GetAll()
    {
        lock (_lock) return _cache.Values.ToList();
    }
}
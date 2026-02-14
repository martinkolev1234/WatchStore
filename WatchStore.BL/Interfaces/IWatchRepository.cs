using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IWatchRepository
{
    IEnumerable<Watch> GetAllWatches();
    Watch? GetWatchById(Guid id);
    void AddWatch(Watch watch);
    void DeleteWatch(Guid id);

    void UpdateWatch(Watch watch);
}
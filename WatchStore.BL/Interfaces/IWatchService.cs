using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IWatchService
{
    IEnumerable<Watch> GetAllWatches();
    Watch? GetWatchById(Guid id);
    Watch AddWatch(Watch watch); 
    void DeleteWatch(Guid id);
    void UpdateWatch(Guid id, UpdateWatchRequest request);
}
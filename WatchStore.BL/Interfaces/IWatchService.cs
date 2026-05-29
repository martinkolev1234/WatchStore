using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IWatchService
{
    Task<IEnumerable<Watch>> GetAllWatchesAsync();
    Task<Watch?> GetWatchByIdAsync(Guid id);
    Task<Watch> AddWatchAsync(Watch watch);
    Task DeleteWatchAsync(Guid id);
    Task UpdateWatchAsync(Guid id, UpdateWatchRequest request);
}
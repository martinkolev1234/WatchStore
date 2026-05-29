using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IWatchRepository
{
    Task<IEnumerable<Watch>> GetAllWatchesAsync();
    Task<Watch?> GetWatchByIdAsync(Guid id);
    Task AddWatchAsync(Watch watch);
    Task DeleteWatchAsync(Guid id);
    Task UpdateWatchAsync(Watch watch);
}
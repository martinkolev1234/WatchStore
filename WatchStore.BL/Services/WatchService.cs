using Mapster;
using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public class WatchService(IWatchRepository watchRepository) : IWatchService
{
    public async Task<IEnumerable<Watch>> GetAllWatchesAsync()
    {
        var watches = await watchRepository.GetAllWatchesAsync();
        return watches.ToList();
    }

    public async Task<Watch?> GetWatchByIdAsync(Guid id)
        => await watchRepository.GetWatchByIdAsync(id);

    public async Task<Watch> AddWatchAsync(Watch watch)
    {
        ArgumentNullException.ThrowIfNull(watch);

        if (watch.Id == Guid.Empty)
            watch.Id = Guid.NewGuid();

        await watchRepository.AddWatchAsync(watch);
        return watch;
    }

    public async Task DeleteWatchAsync(Guid id)
        => await watchRepository.DeleteWatchAsync(id);

    public async Task UpdateWatchAsync(Guid id, UpdateWatchRequest request)
    {
        var existingWatch = await watchRepository.GetWatchByIdAsync(id);

        if (existingWatch is null)
            throw new KeyNotFoundException($"Watch with ID {id} not found.");

        request.Adapt(existingWatch);

        await watchRepository.UpdateWatchAsync(existingWatch);
    }
}
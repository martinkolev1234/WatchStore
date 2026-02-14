using Mapster;
using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

internal class WatchService(IWatchRepository watchRepository) : IWatchService
{
    public IEnumerable<Watch> GetAllWatches()
        => watchRepository.GetAllWatches().ToList();

    public Watch? GetWatchById(Guid id)
        => watchRepository.GetWatchById(id);

    public Watch AddWatch(Watch watch)
    {
        ArgumentNullException.ThrowIfNull(watch);

        if (watch.Id == Guid.Empty)
            watch.Id = Guid.NewGuid();

        watchRepository.AddWatch(watch);
        return watch;
    }

    public void DeleteWatch(Guid id)
        => watchRepository.DeleteWatch(id);

    public void UpdateWatch(Guid id, UpdateWatchRequest request)
    {
        var existingWatch = watchRepository.GetWatchById(id);

        if (existingWatch is null)
            throw new KeyNotFoundException($"Watch with ID {id} not found.");

        request.Adapt(existingWatch);

        watchRepository.UpdateWatch(existingWatch);
    }
}
using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.Kafka;

namespace WatchStore.DL.CacheReader;

public class WatchCacheReaderService(
    IWatchRepository watchRepo,
    KafkaProducer<string, Watch> producer) : IWatchCacheReaderService
{
    public async Task ReadAndPublishAsync(CancellationToken cancellationToken)
    {
        var watches = await watchRepo.GetAllWatchesAsync();
        foreach (var watch in watches)
        {
            await producer.ProduceAsync(watch.Id.ToString(), watch);
        }
    }
}
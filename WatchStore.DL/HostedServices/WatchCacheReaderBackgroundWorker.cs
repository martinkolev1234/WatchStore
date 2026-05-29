using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WatchStore.DL.CacheReader;

namespace WatchStore.DL.HostedServices;

public class WatchCacheReaderBackgroundWorker(
    IWatchCacheReaderService service,
    IConfiguration config) : BackgroundService
{
    private readonly int _intervalSeconds = config.GetValue<int>("DbCacheReader:IntervalSeconds", 10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await service.ReadAndPublishAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
            await service.ReadAndPublishAsync(stoppingToken);
        }
    }
}
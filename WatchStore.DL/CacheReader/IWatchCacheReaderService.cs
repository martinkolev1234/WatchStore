namespace WatchStore.DL.CacheReader;

public interface IWatchCacheReaderService
{
    Task ReadAndPublishAsync(CancellationToken cancellationToken);
}
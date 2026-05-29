using WatchStore.Core;

namespace WatchStore.BL.Services;

public interface IStoreService
{
    Task PurchaseWatchAsync(PurchaseRequest request);
    Task SellWatchToStoreAsync(SellRequest request);
}
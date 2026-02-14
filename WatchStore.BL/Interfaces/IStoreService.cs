using WatchStore.Core;

namespace WatchStore.BL.Services;

public interface IStoreService
{
    void PurchaseWatch(PurchaseRequest request);

    void SellWatchToStore(SellRequest request);
}
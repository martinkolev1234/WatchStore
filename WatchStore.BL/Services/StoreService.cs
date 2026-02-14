using Mapster;
using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public class StoreService(
    IClientRepository clientRepo,
    IWatchRepository watchRepo) : IStoreService
{
    public void PurchaseWatch(PurchaseRequest request)
    {
        var client = clientRepo.GetClientById(request.ClientId);
        var watch = watchRepo.GetWatchById(request.WatchId);

        if (client is null) throw new KeyNotFoundException("Client not found.");
        if (watch is null) throw new KeyNotFoundException("Watch not found.");
        if (watch.OwnerId != null) throw new InvalidOperationException("Watch is already sold.");

        if (client.Balance < watch.Price)
            throw new InvalidOperationException($"Insufficient funds. Need {watch.Price}, have {client.Balance}.");

        client.Balance -= watch.Price;
        watch.OwnerId = client.Id;

        try
        {
            clientRepo.UpdateClient(client);

            watchRepo.UpdateWatch(watch);
        }
        catch (Exception)
        {
            client.Balance += watch.Price;
            clientRepo.UpdateClient(client);

            throw new Exception("Transaction failed. Money refunded.");
        }
    }

    public void SellWatchToStore(SellRequest request)
    {
        var client = clientRepo.GetClientById(request.ClientId);
        if (client is null) throw new KeyNotFoundException("Client not found.");

        var newWatch = request.WatchDetails.Adapt<Watch>();
        newWatch.Id = Guid.NewGuid();
        newWatch.OwnerId = null; 

        client.Balance += newWatch.Price;

        watchRepo.AddWatch(newWatch);
        clientRepo.UpdateClient(client);
    }
}
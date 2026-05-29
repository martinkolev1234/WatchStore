using Mapster;
using WatchStore.Core;
using WatchStore.Core.Models;
using WatchStore.DL.Kafka.Messages;

namespace WatchStore.BL.Services;

public class StoreService(
    IClientRepository clientRepo,
    IWatchRepository watchRepo,
    IKafkaProducer<string, WatchTransactionMessage> kafkaProducer) : IStoreService
{
    public async Task PurchaseWatchAsync(PurchaseRequest request)
    {
        var client = await clientRepo.GetClientByIdAsync(request.ClientId);
        var watch = await watchRepo.GetWatchByIdAsync(request.WatchId);

        if (client is null) throw new KeyNotFoundException("Client not found.");
        if (watch is null) throw new KeyNotFoundException("Watch not found.");
        if (watch.OwnerId != null) throw new InvalidOperationException("Watch is already sold.");

        if (client.Balance < watch.Price)
            throw new InvalidOperationException($"Insufficient funds. Need {watch.Price}, have {client.Balance}.");

        client.Balance -= watch.Price;
        watch.OwnerId = client.Id;

        try
        {
            await clientRepo.UpdateClientAsync(client);
            await watchRepo.UpdateWatchAsync(watch);

            var msg = new WatchTransactionMessage
            {
                WatchId = watch.Id,
                ClientId = client.Id,
                Price = watch.Price,
                TransactionType = "Purchase"
            };
            await kafkaProducer.ProduceAsync(watch.Id.ToString(), msg);
        }
        catch (Exception)
        {
            client.Balance += watch.Price;
            await clientRepo.UpdateClientAsync(client);
            throw new Exception("Transaction failed. Money refunded.");
        }
    }

    public async Task SellWatchToStoreAsync(SellRequest request)
    {
        var client = await clientRepo.GetClientByIdAsync(request.ClientId);
        if (client is null) throw new KeyNotFoundException("Client not found.");

        var newWatch = request.WatchDetails.Adapt<Watch>();
        newWatch.Id = Guid.NewGuid();
        newWatch.OwnerId = null;

        client.Balance += newWatch.Price;

        await watchRepo.AddWatchAsync(newWatch);
        await clientRepo.UpdateClientAsync(client);

        var msg = new WatchTransactionMessage
        {
            WatchId = newWatch.Id,
            ClientId = client.Id,
            Price = newWatch.Price,
            TransactionType = "SellToStore"
        };
        await kafkaProducer.ProduceAsync(newWatch.Id.ToString(), msg);
    }
}
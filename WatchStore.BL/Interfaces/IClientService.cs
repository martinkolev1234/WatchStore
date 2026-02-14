using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IClientService
{
    IEnumerable<Client> GetAllClients();
    Client? GetClientById(Guid id);

    Client AddClient(Client client);

    void DeleteClient(Guid id);

    void UpdateClient(Guid id, UpdateClientRequest request);

    void AddFunds(Guid id, decimal amount);
}
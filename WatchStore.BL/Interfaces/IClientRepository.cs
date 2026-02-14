using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IClientRepository
{
    IEnumerable<Client> GetAllClients();
    Client? GetClientById(Guid id);
    void AddClient(Client client);
    void DeleteClient(Guid id);
    void UpdateClient(Client client);
    Client? GetByEmail(string email);
    bool ExistsByEmail(string email);
    void AddFunds(Guid clientId, decimal amount);
}
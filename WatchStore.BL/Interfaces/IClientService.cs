using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(Guid id);
    Task<Client> AddClientAsync(Client client);
    Task DeleteClientAsync(Guid id);
    Task UpdateClientAsync(Guid id, UpdateClientRequest request);
    Task AddFundsAsync(Guid id, decimal amount);
}
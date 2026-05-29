using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(Guid id);
    Task AddClientAsync(Client client);
    Task DeleteClientAsync(Guid id);
    Task UpdateClientAsync(Client client);
    Task<Client?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddFundsAsync(Guid clientId, decimal amount);
}
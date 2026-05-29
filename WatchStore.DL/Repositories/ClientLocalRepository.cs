using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.StaticDataBase;

namespace WatchStore.DL.Repositories;

internal class ClientLocalRepository : IClientRepository
{
    private static readonly object _lock = new();

    public Task<Client?> GetByEmailAsync(string email)
    {
        Client? result;
        lock (_lock)
        {
            result = StaticDb.Clients.FirstOrDefault(c => c.Email == email);
        }
        return Task.FromResult(result);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        bool exists;
        lock (_lock)
        {
            exists = StaticDb.Clients.Any(c => c.Email == email);
        }
        return Task.FromResult(exists);
    }

    public Task AddFundsAsync(Guid clientId, decimal amount)
    {
        lock (_lock)
        {
            var client = StaticDb.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null)
            {
                client.Balance += amount;
            }
        }
        return Task.CompletedTask;
    }

    public Task AddClientAsync(Client client)
    {
        lock (_lock)
        {
            StaticDb.Clients.Add(client);
        }
        return Task.CompletedTask;
    }

    public Task DeleteClientAsync(Guid id)
    {
        lock (_lock)
        {
            StaticDb.Clients.RemoveAll(x => x.Id == id);
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        IEnumerable<Client> result;
        lock (_lock)
        {
            result = StaticDb.Clients.ToList();
        }
        return Task.FromResult(result);
    }

    public Task<Client?> GetClientByIdAsync(Guid id)
    {
        Client? result;
        lock (_lock)
        {
            result = StaticDb.Clients.FirstOrDefault(x => x.Id == id);
        }
        return Task.FromResult(result);
    }

    public Task UpdateClientAsync(Client client)
    {
        lock (_lock)
        {
            var index = StaticDb.Clients.FindIndex(x => x.Id == client.Id);
            if (index != -1)
            {
                StaticDb.Clients[index] = client;
            }
        }
        return Task.CompletedTask;
    }
}
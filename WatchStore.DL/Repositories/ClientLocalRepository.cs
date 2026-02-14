using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.StaticDataBase;

namespace WatchStore.DL.Repositories;

internal class ClientLocalRepository : IClientRepository
{
    private static readonly object _lock = new();

    public Client? GetByEmail(string email)
    {
        lock (_lock)
        {
            return StaticDb.Clients.FirstOrDefault(c => c.Email == email);
        }
    }

    public bool ExistsByEmail(string email)
    {
        lock (_lock)
        {
            return StaticDb.Clients.Any(c => c.Email == email);
        }
    }

    public void AddFunds(Guid clientId, decimal amount)
    {
        lock (_lock)
        {
            var client = StaticDb.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null)
            {
                client.Balance += amount;
            }
        }
    }

    public void AddClient(Client client)
    {
        lock (_lock)
        {
            StaticDb.Clients.Add(client);
        }
    }

    public void DeleteClient(Guid id)
    {
        lock (_lock)
        {
            StaticDb.Clients.RemoveAll(x => x.Id == id);
        }
    }

    public IEnumerable<Client> GetAllClients()
    {
        lock (_lock)
        {
            return StaticDb.Clients.ToList();
        }
    }

    public Client? GetClientById(Guid id)
    {
        lock (_lock)
        {
            return StaticDb.Clients.FirstOrDefault(x => x.Id == id);
        }
    }

    public void UpdateClient(Client client)
    {
        lock (_lock)
        {
            var index = StaticDb.Clients.FindIndex(x => x.Id == client.Id);
            if (index != -1)
            {
                StaticDb.Clients[index] = client;
            }
        }
    }
}
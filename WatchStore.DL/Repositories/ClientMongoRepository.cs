using MongoDB.Driver;
using WatchStore.BL.Services;
using WatchStore.Core.Models;

namespace WatchStore.DL.Repositories;

internal class ClientRepository : IClientRepository
{
    private readonly IMongoCollection<Client> _collection;

    public ClientRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Client>("Clients");
        InitializeIndexes();
    }

    private void InitializeIndexes()
    {
        try
        {
            var indexKeys = Builders<Client>.IndexKeys.Ascending(c => c.Email);
            var indexOptions = new CreateIndexOptions { Unique = true };
            _collection.Indexes.CreateOne(new CreateIndexModel<Client>(indexKeys, indexOptions));
        }
        catch (Exception)
        {
            Console.WriteLine("Warning: Could not create indexes (MongoDB might be down).");
        }
    }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _collection.Find(c => c.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _collection.Find(c => c.Email == email).AnyAsync();
    }

    public async Task AddFundsAsync(Guid clientId, decimal amount)
    {
        var filter = Builders<Client>.Filter.Eq(c => c.Id, clientId);
        var update = Builders<Client>.Update.Inc(c => c.Balance, amount);
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task AddClientAsync(Client client)
        => await _collection.InsertOneAsync(client);

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        var clients = await _collection.Find(_ => true).ToListAsync();
        return clients;
    }

    public async Task<Client?> GetClientByIdAsync(Guid id)
        => await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task DeleteClientAsync(Guid id)
        => await _collection.DeleteOneAsync(c => c.Id == id);

    public async Task UpdateClientAsync(Client client)
        => await _collection.ReplaceOneAsync(x => x.Id == client.Id, client);
}
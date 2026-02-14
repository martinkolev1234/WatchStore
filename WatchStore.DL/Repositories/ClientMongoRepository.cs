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

    public Client? GetByEmail(string email)
    {
        return _collection.Find(c => c.Email == email).FirstOrDefault();
    }

    public bool ExistsByEmail(string email)
    {
        return _collection.Find(c => c.Email == email).Any();
    }

    public void AddFunds(Guid clientId, decimal amount)
    {
        var filter = Builders<Client>.Filter.Eq(c => c.Id, clientId);
        var update = Builders<Client>.Update.Inc(c => c.Balance, amount);
        _collection.UpdateOne(filter, update);
    }

    public void AddClient(Client client)
        => _collection.InsertOne(client);

    public IEnumerable<Client> GetAllClients()
        => _collection.Find(_ => true).ToList();

    public Client? GetClientById(Guid id)
        => _collection.Find(c => c.Id == id).FirstOrDefault();

    public void DeleteClient(Guid id)
        => _collection.DeleteOne(c => c.Id == id);

    public void UpdateClient(Client client)
        => _collection.ReplaceOne(x => x.Id == client.Id, client);
}
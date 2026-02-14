using Mapster;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

internal class ClientService(IClientRepository clientRepository) : IClientService
{
    public IEnumerable<Client> GetAllClients() => clientRepository.GetAllClients();

    public Client? GetClientById(Guid id) => clientRepository.GetClientById(id);

    public Client AddClient(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (clientRepository.ExistsByEmail(client.Email))
        {
            throw new InvalidOperationException($"Client with email '{client.Email}' already exists.");
        }

        if (client.Id == Guid.Empty) client.Id = Guid.NewGuid();

        clientRepository.AddClient(client);
        return client;
    }

    public void DeleteClient(Guid id) => clientRepository.DeleteClient(id);

    public void UpdateClient(Guid id, UpdateClientRequest request)
    {
        var existingClient = clientRepository.GetClientById(id);
        if (existingClient is null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        if (!existingClient.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (clientRepository.ExistsByEmail(request.Email))
                throw new InvalidOperationException($"Email '{request.Email}' is already taken.");
        }

        request.Adapt(existingClient);
        clientRepository.UpdateClient(existingClient);
    }

    public void AddFunds(Guid id, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");

        var client = clientRepository.GetClientById(id);
        if (client is null) throw new KeyNotFoundException("Client not found");

        clientRepository.AddFunds(id, amount);
    }
}
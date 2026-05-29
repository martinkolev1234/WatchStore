using Mapster;
using WatchStore.Core.Models;

namespace WatchStore.BL.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    public async Task<IEnumerable<Client>> GetAllClientsAsync() =>
        await clientRepository.GetAllClientsAsync();

    public async Task<Client?> GetClientByIdAsync(Guid id) =>
        await clientRepository.GetClientByIdAsync(id);

    public async Task<Client> AddClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (await clientRepository.ExistsByEmailAsync(client.Email))
        {
            throw new InvalidOperationException($"Client with email '{client.Email}' already exists.");
        }

        if (client.Id == Guid.Empty) client.Id = Guid.NewGuid();

        await clientRepository.AddClientAsync(client);
        return client;
    }

    public async Task DeleteClientAsync(Guid id) =>
        await clientRepository.DeleteClientAsync(id);

    public async Task UpdateClientAsync(Guid id, UpdateClientRequest request)
    {
        var existingClient = await clientRepository.GetClientByIdAsync(id);
        if (existingClient is null)
            throw new KeyNotFoundException($"Client with ID {id} not found.");

        if (!existingClient.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await clientRepository.ExistsByEmailAsync(request.Email))
                throw new InvalidOperationException($"Email '{request.Email}' is already taken.");
        }

        request.Adapt(existingClient);
        await clientRepository.UpdateClientAsync(existingClient);
    }

    public async Task AddFundsAsync(Guid id, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");

        var client = await clientRepository.GetClientByIdAsync(id);
        if (client is null) throw new KeyNotFoundException("Client not found");

        await clientRepository.AddFundsAsync(id, amount);
    }
}
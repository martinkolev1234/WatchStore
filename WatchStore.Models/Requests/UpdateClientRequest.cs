namespace WatchStore.Core.Models;

public record UpdateClientRequest(
    string Name,
    string Email,
    string PhoneNumber,
    string Address
);
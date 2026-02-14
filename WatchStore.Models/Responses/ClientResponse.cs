namespace WatchStore.Core.Responses;

public record ClientResponse(
    Guid Id,
    string Name,
    string Email,
    string Address,
    decimal Balance);
namespace WatchStore.Core.Requests;
public record CreateClientRequest(
    string Name,
    string Email,
    string PhoneNumber,
    string Address);
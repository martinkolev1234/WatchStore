namespace WatchStore.Core.Requests;

public record CreateWatchRequest(
    string Brand,
    string Model,
    decimal Price,
    int ProductionYear,
    decimal CaseDiameterMm);
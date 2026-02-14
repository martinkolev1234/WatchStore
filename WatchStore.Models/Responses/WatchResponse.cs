namespace WatchStore.Core.Responses;

public record WatchResponse(
    Guid Id,
    string Brand,
    string Model,
    decimal Price,
    int ProductionYear,
    decimal CaseDiameterMm
);
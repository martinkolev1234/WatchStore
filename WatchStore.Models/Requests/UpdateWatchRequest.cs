namespace WatchStore.Core;

public record UpdateWatchRequest(
    string Brand,
    string Model,
    decimal Price,
    int ProductionYear,
    decimal CaseDiameterMm
);
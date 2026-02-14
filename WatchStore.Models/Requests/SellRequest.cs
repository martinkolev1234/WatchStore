using WatchStore.Core.Requests;

namespace WatchStore.Core;

public record SellRequest(Guid ClientId, CreateWatchRequest WatchDetails);
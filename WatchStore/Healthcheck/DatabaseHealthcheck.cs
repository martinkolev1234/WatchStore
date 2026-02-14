using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WatchStore.Api.HealthChecks;

public class DatabaseHealthCheck(IMongoDatabase database) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await database.RunCommandAsync(
                (Command<BsonDocument>)"{ping:1}",
                cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB is OK.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB failed.", ex);
        }
    }
}
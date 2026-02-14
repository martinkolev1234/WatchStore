using Microsoft.Extensions.DependencyInjection;
using WatchStore.BL.Services;
using WatchStore.DL.Repositories; 

namespace WatchStore.DL;

public static class DataLayerDependencyInjection
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IWatchRepository, WatchRepository>();

        return services;
    }
}
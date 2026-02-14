using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WatchStore.BL.Services;
using WatchStore.BL.Validators;

namespace WatchStore.BL;

public static class BusinessLayerDependencyInjection
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IWatchService, WatchService>();
        services.AddScoped<IStoreService, StoreService>();

        services.AddValidatorsFromAssemblyContaining<CreateClientRequestValidator>();

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
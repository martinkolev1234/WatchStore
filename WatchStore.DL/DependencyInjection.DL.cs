using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.DL.Cache;
using WatchStore.DL.CacheReader;
using WatchStore.DL.HostedServices;
using WatchStore.DL.Kafka;
using WatchStore.DL.Kafka.Messages;
using WatchStore.DL.Repositories;

namespace WatchStore.DL;

public static class DataLayerDependencyInjection
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configs)
    {
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IWatchRepository, WatchRepository>();

        services.AddSingleton<WatchCache>();

        var kafkaSettings = configs.GetSection("KafkaSettings").Get<KafkaSettings>()
            ?? throw new InvalidOperationException("KafkaSettings missing from appsettings.json");

        services.AddSingleton(kafkaSettings);
        services.Configure<KafkaSettings>(configs.GetSection("KafkaSettings"));

        services.AddSingleton(sp => new KafkaProducer<string, WatchTransactionMessage>(kafkaSettings));

        var cacheKafkaSettings = new KafkaSettings
        {
            BootstrapServers = kafkaSettings.BootstrapServers,
            SaslUsername = kafkaSettings.SaslUsername,
            SaslPassword = kafkaSettings.SaslPassword,
            Topic = configs["DbCacheReader:Topic"] ?? "watches-cache",
            GroupId = kafkaSettings.GroupId
        };
        services.AddSingleton(sp => new KafkaProducer<string, Watch>(cacheKafkaSettings));

        services.AddSingleton<IWatchCacheReaderService, WatchCacheReaderService>();
        services.AddHostedService<WatchCacheReaderBackgroundWorker>();
        services.AddHostedService<KafkaWatchCacheConsumer>();
        services.AddHostedService<KafkaTransactionConsumerWorker>();

        return services;
    }
}
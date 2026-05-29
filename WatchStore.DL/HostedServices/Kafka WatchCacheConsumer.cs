using Confluent.Kafka;
using MessagePack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WatchStore.Core.Models;
using WatchStore.DL.Cache;
using WatchStore.DL.Kafka;

namespace WatchStore.DL.HostedServices;

public class KafkaWatchCacheConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, byte[]> _consumer;
    private readonly WatchCache _cache;
    private readonly ILogger<KafkaWatchCacheConsumer> _logger;
    private readonly IDisposable? _optionsChangeToken;

    public KafkaWatchCacheConsumer(
        WatchCache cache,
        IOptionsMonitor<KafkaSettings> optionsMonitor,
        ILogger<KafkaWatchCacheConsumer> logger)
    {
        _cache = cache;
        _logger = logger;
        var settings = optionsMonitor.CurrentValue;

        var config = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.ScramSha256,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword,
            EnableSslCertificateVerification = false,
            GroupId = "watches-cache-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, byte[]>(config).Build();

        _optionsChangeToken = optionsMonitor.OnChange((newSettings, name) =>
        {
            _logger.LogInformation("KafkaSettings changed for Cache Consumer");
        });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe("watches-cache");

        return Task.Run(() =>
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result?.Message?.Value != null && result.Message.Value.Length > 0)
                    {
                        try
                        {
                            var watch = MessagePackSerializer.Deserialize<Watch>(
                                result.Message.Value,
                                MessagePack.Resolvers.ContractlessStandardResolver.Options);

                            if (watch != null)
                            {
                                _cache.Add(watch);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to deserialize watch message: {ex.Message}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Watch Cache Consumer stopped");
            }
            finally
            {
                _consumer.Close();
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _optionsChangeToken?.Dispose();
        _consumer?.Dispose();
        base.Dispose();
    }
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WatchStore.DL.Kafka;
using WatchStore.DL.Kafka.Messages;

namespace WatchStore.DL.HostedServices;

public class KafkaTransactionConsumerWorker(
    ILogger<KafkaTransactionConsumerWorker> logger,
    KafkaSettings kafkaSettings) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Kafka Transaction Consumer Worker is starting...");

        return Task.Run(() =>
        {
            var consumer = new KafkaConsumer<string, WatchTransactionMessage>(
                kafkaSettings,
                onMessageReceived: (key, msg) =>
                {
                    logger.LogInformation(
                        "\n====== NEW TRANSACTION RECEIVED FROM KAFKA ======\n" +
                        "Type:      {Type}\n" +
                        "Client ID: {ClientId}\n" +
                        "Watch ID:  {WatchId}\n" +
                        "Price:     ${Price}\n" +
                        "=================================================",
                        msg.TransactionType, msg.ClientId, msg.WatchId, msg.Price);
                });

            consumer.StartConsuming(stoppingToken);

        }, stoppingToken);
    }
}
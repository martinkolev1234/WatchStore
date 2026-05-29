using Confluent.Kafka;

namespace WatchStore.DL.Kafka;

public class KafkaConsumer<TKey, TValue> where TValue : class
{
    private readonly KafkaSettings _settings;
    private readonly Action<TKey, TValue> _onMessageReceived;

    public KafkaConsumer(KafkaSettings settings, Action<TKey, TValue> onMessageReceived)
    {
        _settings = settings;
        _onMessageReceived = onMessageReceived;
    }

    public void StartConsuming(CancellationToken token)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = string.IsNullOrWhiteSpace(_settings.GroupId) ? Guid.NewGuid().ToString() : _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.ScramSha256,
            SaslUsername = _settings.SaslUsername,
            SaslPassword = _settings.SaslPassword,
            EnableSslCertificateVerification = false
        };

        using var consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetValueDeserializer(new KafkaMessageDeserializer<TValue>())
            .Build();

        consumer.Subscribe(_settings.Topic);

        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = consumer.Consume(token);

                if (result?.Message?.Value != null)
                {
                    _onMessageReceived(result.Message.Key, result.Message.Value);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            consumer.Close();
        }
    }
}
using Confluent.Kafka;

namespace WatchStore.DL.Kafka;

public class KafkaProducer<TKey, TValue> : BL.Services.IKafkaProducer<TKey, TValue>, IDisposable where TValue : class
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaProducer(KafkaSettings settings)
    {
        _topic = settings.Topic;

        var config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.ScramSha256,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword,
            EnableSslCertificateVerification = false
        };

        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetValueSerializer(new KafkaMessageSerializer<TValue>())
            .Build();
    }

    public async Task ProduceAsync(TKey key, TValue message)
    {
        await _producer.ProduceAsync(_topic, new Message<TKey, TValue>
        {
            Key = key,
            Value = message
        });
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
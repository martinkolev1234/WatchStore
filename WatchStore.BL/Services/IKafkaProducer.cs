namespace WatchStore.BL.Services;

public interface IKafkaProducer<TKey, TValue>
{
    Task ProduceAsync(TKey key, TValue message);
}
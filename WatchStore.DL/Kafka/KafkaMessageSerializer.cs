using Confluent.Kafka;
using MessagePack;

namespace WatchStore.DL.Kafka;

public class KafkaMessageSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
        => MessagePackSerializer.Serialize(data, MessagePack.Resolvers.ContractlessStandardResolver.Options);
}

public class KafkaMessageDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty) return default!;
        return MessagePackSerializer.Deserialize<T>(data.ToArray(), MessagePack.Resolvers.ContractlessStandardResolver.Options);
    }
}
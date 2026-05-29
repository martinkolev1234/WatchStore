using MessagePack;

namespace WatchStore.DL.Kafka.Messages;

[MessagePackObject]
public class WatchTransactionMessage
{
    [Key(0)]
    public Guid WatchId { get; set; }

    [Key(1)]
    public Guid ClientId { get; set; }

    [Key(2)]
    public decimal Price { get; set; }

    [Key(3)]
    public string TransactionType { get; set; } = string.Empty; 
}
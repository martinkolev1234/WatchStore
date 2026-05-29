using Moq;
using WatchStore.BL.Services;
using WatchStore.Core;
using WatchStore.Core.Models;
using WatchStore.Core.Requests;
using WatchStore.DL.Kafka.Messages;

namespace WatchStore.Tests.StoreServiceTests;

public class SellTest
{
    private readonly Mock<IClientRepository> _mockClientRepo = new();
    private readonly Mock<IWatchRepository> _mockWatchRepo = new();
    private readonly Mock<IKafkaProducer<string, WatchTransactionMessage>> _mockKafkaProducer = new();
    private readonly StoreService _service;

    public SellTest()
    {
        _service = new StoreService(_mockClientRepo.Object, _mockWatchRepo.Object, _mockKafkaProducer.Object);
    }

    [Fact]
    public async Task SellWatchToStoreAsync_Should_Increase_Balance()
    {
        var clientId = Guid.NewGuid();
        var client = new Client { Id = clientId, Balance = 100m };
        var watchInfo = new CreateWatchRequest("Casio", "Basic", 50m, 2024, 40);

        _mockClientRepo.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);

        await _service.SellWatchToStoreAsync(new SellRequest(clientId, watchInfo));

        Assert.Equal(150m, client.Balance);

        _mockWatchRepo.Verify(r => r.AddWatchAsync(It.IsAny<Watch>()), Times.Once);

        _mockKafkaProducer.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.Is<WatchTransactionMessage>(msg => msg.TransactionType == "SellToStore")), Times.Once);
    }
}
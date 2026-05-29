using Moq;
using WatchStore.BL.Services;
using WatchStore.Core;
using WatchStore.Core.Models;
using WatchStore.DL.Kafka.Messages;

namespace WatchStore.Tests.StoreServiceTests;

public class PurchaseTest
{
    private readonly Mock<IClientRepository> _mockClientRepo;
    private readonly Mock<IWatchRepository> _mockWatchRepo;
    private readonly Mock<IKafkaProducer<string, WatchTransactionMessage>> _mockKafkaProducer;
    private readonly StoreService _service;

    public PurchaseTest()
    {
        _mockClientRepo = new Mock<IClientRepository>();
        _mockWatchRepo = new Mock<IWatchRepository>();
        _mockKafkaProducer = new Mock<IKafkaProducer<string, WatchTransactionMessage>>();

        _service = new StoreService(_mockClientRepo.Object, _mockWatchRepo.Object, _mockKafkaProducer.Object);
    }

    [Fact]
    public async Task PurchaseWatchAsync_Should_Succeed_When_Funds_Are_Sufficient()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 1000m };
        var watch = new Watch { Id = watchId, Price = 500m, OwnerId = null };

        _mockClientRepo.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);
        _mockWatchRepo.Setup(r => r.GetWatchByIdAsync(watchId)).ReturnsAsync(watch);

        var request = new PurchaseRequest(clientId, watchId);

        await _service.PurchaseWatchAsync(request);

        Assert.Equal(500m, client.Balance);
        Assert.Equal(clientId, watch.OwnerId);

        _mockClientRepo.Verify(r => r.UpdateClientAsync(client), Times.Once);
        _mockWatchRepo.Verify(r => r.UpdateWatchAsync(watch), Times.Once);

        _mockKafkaProducer.Verify(k => k.ProduceAsync(watchId.ToString(), It.IsAny<WatchTransactionMessage>()), Times.Once);
    }

    [Fact]
    public async Task PurchaseWatchAsync_Should_Throw_When_Insufficient_Funds()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 100m };
        var watch = new Watch { Id = watchId, Price = 5000m, OwnerId = null };

        _mockClientRepo.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);
        _mockWatchRepo.Setup(r => r.GetWatchByIdAsync(watchId)).ReturnsAsync(watch);

        var request = new PurchaseRequest(clientId, watchId);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.PurchaseWatchAsync(request));

        Assert.Contains("Insufficient funds", exception.Message);

        _mockClientRepo.Verify(r => r.UpdateClientAsync(It.IsAny<Client>()), Times.Never);

        _mockKafkaProducer.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<WatchTransactionMessage>()), Times.Never);
    }

    [Fact]
    public async Task PurchaseWatchAsync_Should_Throw_When_Watch_Is_Already_Sold()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 99999m };

        var watch = new Watch { Id = watchId, Price = 500m, OwnerId = Guid.NewGuid() };

        _mockClientRepo.Setup(r => r.GetClientByIdAsync(clientId)).ReturnsAsync(client);
        _mockWatchRepo.Setup(r => r.GetWatchByIdAsync(watchId)).ReturnsAsync(watch);

        var request = new PurchaseRequest(clientId, watchId);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.PurchaseWatchAsync(request));
        Assert.Equal("Watch is already sold.", ex.Message);

        _mockKafkaProducer.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<WatchTransactionMessage>()), Times.Never);
    }
}
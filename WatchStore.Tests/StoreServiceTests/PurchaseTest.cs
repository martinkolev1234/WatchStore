using Moq;
using WatchStore.BL.Services;
using WatchStore.Core.Models;
using WatchStore.Core;

namespace WatchStore.Tests;

public class StoreServiceTests
{
    private readonly Mock<IClientRepository> _mockClientRepo;
    private readonly Mock<IWatchRepository> _mockWatchRepo;
    private readonly StoreService _service;

    public StoreServiceTests()
    {
        _mockClientRepo = new Mock<IClientRepository>();
        _mockWatchRepo = new Mock<IWatchRepository>();

        _service = new StoreService(_mockClientRepo.Object, _mockWatchRepo.Object);
    }

    [Fact]
    public void PurchaseWatch_Should_Succeed_When_Funds_Are_Sufficient()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 1000m };
        var watch = new Watch { Id = watchId, Price = 500m, OwnerId = null };

        _mockClientRepo.Setup(r => r.GetClientById(clientId)).Returns(client);
        _mockWatchRepo.Setup(r => r.GetWatchById(watchId)).Returns(watch);

        var request = new PurchaseRequest(clientId, watchId);

        _service.PurchaseWatch(request);

        Assert.Equal(500m, client.Balance); 
        Assert.Equal(clientId, watch.OwnerId); 

        _mockClientRepo.Verify(r => r.UpdateClient(client), Times.Once);
        _mockWatchRepo.Verify(r => r.UpdateWatch(watch), Times.Once);
    }

    [Fact]
    public void PurchaseWatch_Should_Throw_When_Insufficient_Funds()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 100m }; 
        var watch = new Watch { Id = watchId, Price = 5000m, OwnerId = null }; 

        _mockClientRepo.Setup(r => r.GetClientById(clientId)).Returns(client);
        _mockWatchRepo.Setup(r => r.GetWatchById(watchId)).Returns(watch);

        var request = new PurchaseRequest(clientId, watchId);

        var exception = Assert.Throws<InvalidOperationException>(() => _service.PurchaseWatch(request));

        Assert.Contains("Insufficient funds", exception.Message);

        _mockClientRepo.Verify(r => r.UpdateClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void PurchaseWatch_Should_Throw_When_Watch_Is_Already_Sold()
    {
        var clientId = Guid.NewGuid();
        var watchId = Guid.NewGuid();

        var client = new Client { Id = clientId, Balance = 99999m };
        
        var watch = new Watch { Id = watchId, Price = 500m, OwnerId = Guid.NewGuid() };

        _mockClientRepo.Setup(r => r.GetClientById(clientId)).Returns(client);
        _mockWatchRepo.Setup(r => r.GetWatchById(watchId)).Returns(watch);

        var request = new PurchaseRequest(clientId, watchId);

        var ex = Assert.Throws<InvalidOperationException>(() => _service.PurchaseWatch(request));
        Assert.Equal("This watch is already sold.", ex.Message);
    }
}
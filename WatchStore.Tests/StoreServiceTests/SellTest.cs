using Moq;
using WatchStore.BL.Services;
using WatchStore.Core;
using WatchStore.Core.Models;
using WatchStore.Core.Requests; 

namespace WatchStore.Tests;

public class SellServiceTests
{
    private readonly Mock<IClientRepository> _mockClientRepo = new();
    private readonly Mock<IWatchRepository> _mockWatchRepo = new();
    private readonly StoreService _service;

    public SellServiceTests()
    {
        _service = new StoreService(_mockClientRepo.Object, _mockWatchRepo.Object);
    }

    [Fact]
    public void Sell_Should_Increase_Balance()
    {
        var clientId = Guid.NewGuid();
        var client = new Client { Id = clientId, Balance = 100m };
        var watchInfo = new CreateWatchRequest("Casio", "Basic", 50m, 2024, 40);

        _mockClientRepo.Setup(r => r.GetClientById(clientId)).Returns(client);

        _service.SellWatchToStore(new SellRequest(clientId, watchInfo));

        Assert.Equal(150m, client.Balance); 

        _mockWatchRepo.Verify(r => r.AddWatch(It.IsAny<Watch>()), Times.Once);
    }
}
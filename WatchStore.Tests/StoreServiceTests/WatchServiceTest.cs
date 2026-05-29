using Moq;
using WatchStore.BL.Services;
using WatchStore.Core;
using WatchStore.Core.Models;

namespace WatchStore.Tests.StoreServiceTests;

public class WatchServiceTests
{
    private readonly Mock<IWatchRepository> _mockRepo;
    private readonly WatchService _service;

    public WatchServiceTests()
    {
        _mockRepo = new Mock<IWatchRepository>();
        _service = new WatchService(_mockRepo.Object);
    }

    [Fact]
    public async Task UpdateWatchAsync_Should_Throw_KeyNotFound_If_Id_Does_Not_Exist()
    {
        var watchId = Guid.NewGuid();

        _mockRepo.Setup(r => r.GetWatchByIdAsync(watchId)).ReturnsAsync((Watch?)null);

        var updateRequest = new UpdateWatchRequest("Rolex", "Daytona", 20000, 2022, 40);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateWatchAsync(watchId, updateRequest));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public async Task UpdateWatchAsync_Should_Update_Fields_Correctly()
    {
        var watchId = Guid.NewGuid();
        var existingWatch = new Watch
        {
            Id = watchId,
            Brand = "OldBrand",
            Price = 100m
        };

        _mockRepo.Setup(r => r.GetWatchByIdAsync(watchId)).ReturnsAsync(existingWatch);

        var updateRequest = new UpdateWatchRequest("NewBrand", "NewModel", 500m, 2023, 41);

        await _service.UpdateWatchAsync(watchId, updateRequest);

        Assert.Equal("NewBrand", existingWatch.Brand);
        Assert.Equal(500m, existingWatch.Price);

        _mockRepo.Verify(r => r.UpdateWatchAsync(existingWatch), Times.Once);
    }
}
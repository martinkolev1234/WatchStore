using Moq;
using WatchStore.BL.Services;
using WatchStore.Core.Models;

namespace WatchStore.Tests.ClientServiceTest;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _mockRepo;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        _mockRepo = new Mock<IClientRepository>();
        _service = new ClientService(_mockRepo.Object);
    }

    [Fact]
    public async Task AddClientAsync_Should_Throw_If_Email_Exists()
    {
        var existingEmail = "test@duplicate.com";

        _mockRepo.Setup(r => r.ExistsByEmailAsync(existingEmail)).ReturnsAsync(true);

        var newClient = new Client { Name = "New Guy", Email = existingEmail };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddClientAsync(newClient));

        Assert.Contains("already exists", ex.Message);
        _mockRepo.Verify(r => r.AddClientAsync(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public async Task AddClientAsync_Should_Add_If_Email_Is_Unique()
    {
        _mockRepo.Setup(r => r.ExistsByEmailAsync("unique@mail.com")).ReturnsAsync(false);

        var newClient = new Client { Name = "New Guy", Email = "unique@mail.com" };

        await _service.AddClientAsync(newClient);

        _mockRepo.Verify(r => r.AddClientAsync(newClient), Times.Once);
    }
}
using Moq;
using WatchStore.BL.Services;
using WatchStore.Core.Models;

namespace WatchStore.Tests;

public class ClientServiceTests
{
    private readonly Mock<IClientRepository> _mockRepo;
    private readonly IClientService _service;

    public ClientServiceTests()
    {
        _mockRepo = new Mock<IClientRepository>();
        _service = new ClientService(_mockRepo.Object);
    }

    [Fact]
    public void AddClient_Should_Throw_If_Email_Exists()
    {
        var existingEmail = "test@duplicate.com";

        var existingClients = new List<Client>
        {
            new Client { Id = Guid.NewGuid(), Email = existingEmail }
        };

        _mockRepo.Setup(r => r.GetAllClients()).Returns(existingClients);

        var newClient = new Client { Name = "New Guy", Email = existingEmail };

        var ex = Assert.Throws<InvalidOperationException>(() => _service.AddClient(newClient));

        Assert.Contains("already exists", ex.Message);
        _mockRepo.Verify(r => r.AddClient(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void AddClient_Should_Add_If_Email_Is_Unique()
    {
        _mockRepo.Setup(r => r.GetAllClients()).Returns(new List<Client>()); 

        var newClient = new Client { Name = "New Guy", Email = "unique@mail.com" };

        _service.AddClient(newClient);

        _mockRepo.Verify(r => r.AddClient(newClient), Times.Once);
    }
}
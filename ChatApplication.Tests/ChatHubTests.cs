using ChatApplication.Server.Data.Context;
using ChatApplication.Server.Data.Entities;
using ChatApplication.Server.Encryption;
using ChatApplication.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace ChatApplication.Tests;

public class ChatHubTests
{
    private readonly Mock<IEncryptionService> _mockEncryptionService;
    private readonly Mock<ILogger<ChatHub>> _mockLogger;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public ChatHubTests()
    {
        _mockEncryptionService = new Mock<IEncryptionService>();
        _mockLogger = new Mock<ILogger<ChatHub>>();
        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private ChatHub CreateChatHub(ApplicationDbContext context, string userId, string displayName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new("displayName", displayName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var mockClients = new Mock<IHubCallerClients>();
        var mockGroupManager = new Mock<IGroupManager>();
        var mockClientProxy = new Mock<IClientProxy>();
        var mockHubCallerContext = new Mock<HubCallerContext>();

        mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);
        mockHubCallerContext.Setup(c => c.User).Returns(user);
        mockHubCallerContext.Setup(c => c.ConnectionId).Returns(Guid.NewGuid().ToString());

        var hub = new ChatHub(context, _mockLogger.Object, _mockEncryptionService.Object)
        {
            Clients = mockClients.Object,
            Groups = mockGroupManager.Object,
            Context = mockHubCallerContext.Object
        };

        return hub;
    }

    [Fact]
    public async Task SendMessage_WithValidMessage_SavesMessageAndSendsToGroup()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbOptions);
        const int roomId = 1;
        const string userId = "test-user-id";
        const string message = "Hello, world!";

        context.ChatRooms.Add(new ChatRoomEntity { Id = roomId, Name = "Public Room", IsPrivate = false });
        await context.SaveChangesAsync();

        var hub = CreateChatHub(context, userId, "Test User");
        _mockEncryptionService.Setup(e => e.Encrypt(It.IsAny<string>())).Returns((string s) => s);

        // Act
        await hub.SendMessage(roomId, message);

        // Assert
        var savedMessage = await context.Messages.FirstOrDefaultAsync();
        Assert.NotNull(savedMessage);
        Assert.Equal(message, savedMessage.Content);
        Assert.Equal(userId, savedMessage.UserId);
    }

    [Fact]
    public async Task SendMessage_WithEmptyMessage_ThrowsHubException()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbOptions);
        const int roomId = 1;
        context.ChatRooms.Add(new ChatRoomEntity { Id = roomId, Name = "Test Room", IsPrivate = false });
        await context.SaveChangesAsync();

        var hub = CreateChatHub(context, "test-user-id", "Test User");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HubException>(() => hub.SendMessage(roomId, string.Empty));
        Assert.Equal("Message cannot be empty", exception.Message);
    }

    [Fact]
    public async Task JoinRoom_WhenUserIsAuthorized_AddsUserToGroup()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbOptions);
        const int roomId = 1;
        const string userId = "test-user-id";

        context.ChatRooms.Add(new ChatRoomEntity { Id = roomId, Name = "Public Room", IsPrivate = false });
        await context.SaveChangesAsync();

        var hub = CreateChatHub(context, userId, "Test User");

        // Act
        await hub.JoinRoom(roomId);

        // Assert
        Mock.Get(hub.Groups).Verify(g => g.AddToGroupAsync(It.IsAny<string>(), $"room_{roomId}", It.IsAny<CancellationToken>()), Times.Once);
    }
}

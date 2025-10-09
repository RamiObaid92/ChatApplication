using ChatApplication.Server.Data.Context;
using ChatApplication.Server.Data.Entities;
using ChatApplication.Server.Encryption;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApplication.Server.Hubs;

[Authorize]
public class ChatHub(ApplicationDbContext dbContext, ILogger<ChatHub> logger, IEncryptionService encryptionService) : Hub
{
    private readonly ApplicationDbContext _db = dbContext;
    private readonly HtmlSanitizer _sanitizer = new();
    private readonly ILogger<ChatHub> _logger = logger;
    private readonly IEncryptionService _encryption = encryptionService;

    public async Task JoinRoom(int roomId)
    {
        var userId = GetUserId();

        if (!await IsUserAuthorizedForRoom(userId, roomId))
        {
            throw new HubException("Access denied: You are not a member of this private room.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        _logger.LogInformation("User {UserId} joined room {RoomId}", userId, roomId);

        var displayName = GetUserDisplayName();
        await Clients.Group(RoomGroup(roomId)).SendAsync("UserJoined", new
        {
            UserId = userId,
            DisplayName = displayName,
            RoomId = roomId
        });
    }

    public async Task LeaveRoom(int roomId)
    {
        var userId = GetUserId();
        var displayName = GetUserDisplayName();

        await Clients.OthersInGroup(RoomGroup(roomId)).SendAsync("UserLeft", new
        {
            UserId = userId,
            DisplayName = displayName,
            RoomId = roomId
        });
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        _logger.LogInformation("User {UserId} left room {RoomId}", userId, roomId);
    }

    public async Task SendMessage(int roomId, string message)
    {
        var userId = GetUserId();
        if (!await IsUserAuthorizedForRoom(userId, roomId))
        {
            _logger.LogWarning("Unauthorized user {UserId} attempted to send a message to room {RoomId}", userId, roomId);
            throw new HubException("Access denied to this room.");
        }

        var sanitizedMessage = SanitizeAndValidateMessage(message);
        var encryptedMessage = _encryption.Encrypt(sanitizedMessage);
        var messageEntity = new ChatMessageEntity
        {
            Content = encryptedMessage,
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            ChatRoomId = roomId
        };

        _db.Messages.Add(messageEntity);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Message {MessageId} sent to room {RoomId} by user {UserId}", messageEntity.Id, roomId, userId);

        var displayName = GetUserDisplayName();
        await Clients.Group(RoomGroup(roomId)).SendAsync("ReceiveMessage", new
        {
            messageEntity.Id,
            Content = sanitizedMessage,
            messageEntity.Timestamp,
            User = new { Id = userId, DisplayName = displayName },
            RoomId = roomId
        });
    }

    private async Task<bool> IsUserAuthorizedForRoom(string userId, int roomId)
    {
        return await _db.ChatRooms
            .Where(r => r.Id == roomId)
            .AnyAsync(r => !r.IsPrivate || r.Members.Any(m => m.Id == userId));
    }

    private string SanitizeAndValidateMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new HubException("Message cannot be empty");

        if (message.Length > 2000)
            throw new HubException("Message is too long (max 2000 characters)");

        return _sanitizer.Sanitize(message);
    }

    private string GetUserId()
        => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new HubException("User not authenticated");

    private string GetUserDisplayName()
        => Context.User?.FindFirstValue("displayName")
           ?? Context.User?.Identity?.Name
           ?? "Unknown";

    private static string RoomGroup(int roomId) => $"room_{roomId}";
}

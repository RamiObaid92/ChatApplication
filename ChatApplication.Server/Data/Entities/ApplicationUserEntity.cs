using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Data.Entities;

public class ApplicationUserEntity : IdentityUser
{
    public string? DisplayName { get; set; }

    public ICollection<ChatMessageEntity> Messages { get; set; } = [];
    public ICollection<ChatRoomEntity> ChatRooms { get; set; } = [];
}

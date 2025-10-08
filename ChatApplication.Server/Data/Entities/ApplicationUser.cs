using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<ChatMessage> Messages { get; set; } = [];
    public ICollection<ChatRoom> ChatRooms { get; set; } = [];
}

using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Server.Data.Entities;

public class ChatRoomEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = false;
    public ICollection<ChatMessageEntity> Messages { get; set; } = [];
    public ICollection<ApplicationUserEntity> Members { get; set; } = [];
}

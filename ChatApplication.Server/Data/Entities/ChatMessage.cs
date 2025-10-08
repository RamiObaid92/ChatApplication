using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApplication.Server.Data.Entities;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }
    public int ChatRoomId { get; set; }

    [ForeignKey(nameof(ChatRoomId))]
    public ChatRoom? ChatRoom { get; set; }
}

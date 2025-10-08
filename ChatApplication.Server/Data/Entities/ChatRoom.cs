using System.ComponentModel.DataAnnotations;

namespace ChatApplication.Server.Data.Entities;

public class ChatRoom
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public ICollection<ChatMessage> Messages { get; set; } = [];
    public ICollection<ApplicationUser> Members { get; set; } = [];
}

using ChatApplication.Server.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Data.Context;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ChatMessage> Messages { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
}

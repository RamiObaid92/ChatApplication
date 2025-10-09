using ChatApplication.Server.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApplication.Server.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms").RequireAuthorization();

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var allRooms = await db.ChatRooms
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.IsPrivate
            }).ToListAsync();

            return Results.Ok(allRooms);
        });

        group.MapGet("/{roomId:int}/messages", async (int roomId, ClaimsPrincipal user, ApplicationDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var hasAccess = await db.ChatRooms
                .Where(r => r.Id == roomId)
                .AnyAsync(r => !r.IsPrivate || r.Members.Any(m => m.Id == userId));

            if (!hasAccess)
            {
                return Results.NotFound(new { error = "Room not found or access denied" });
            }

            var messages = await db.Messages
                .Where(m => m.ChatRoomId == roomId)
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.Timestamp,
                    User = new
                    {
                        m.User!.Id,
                        DisplayName = m.User!.DisplayName ?? m.User!.UserName
                    }
                })
                .ToListAsync();
            return Results.Ok(messages.OrderBy(m => m.Timestamp));
        });
    }
}

using ChatApplication.Server.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Endpoints;

public static class RoomEndpoints
{
    public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rooms");

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var rooms = await db.ChatRooms
            .Select(r => new
            {
                r.Id,
                r.Name
            }).ToListAsync();

            return Results.Ok(rooms);
        });
    }
}

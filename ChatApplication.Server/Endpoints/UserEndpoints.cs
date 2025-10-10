using ChatApplication.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ChatApplication.Server.Endpoints;

public record UpdateDisplayNameRequest(string DisplayName);

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/user").RequireAuthorization();

        group.MapPut("/displayname", async (
            UpdateDisplayNameRequest request,
            ClaimsPrincipal principal,
            UserManager<ApplicationUserEntity> userManager) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }
            var user = await userManager.FindByIdAsync(userId);

            if (user == null) { return Results.NotFound("User not found."); }

            if (string.IsNullOrWhiteSpace(request.DisplayName))
            {
                return Results.BadRequest("Display name is invalid.");
            }

            user.DisplayName = request.DisplayName;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded) { return Results.BadRequest(result.Errors); }

            return Results.Ok(new { message = "Display name updated successfully." });
        });
    }
}

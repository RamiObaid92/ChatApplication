using ChatApplication.Server.Data.Entities;
using ChatApplication.Server.Services;
using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Endpoints;

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, UserManager<ApplicationUserEntity> userManager) =>
        {
            var user = new ApplicationUserEntity { UserName = request.Username, Email = request.Username };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(new { Message = "User registered successfully" });
        });

        group.MapPost("/login", async (LoginRequest request, UserManager<ApplicationUserEntity> userManager, ITokenService tokenService) =>
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }

            var jwt = tokenService.GenerateJwtToken(user);
            return Results.Ok(new { Token = jwt });
        });
    }
}

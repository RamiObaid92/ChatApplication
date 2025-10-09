using ChatApplication.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace ChatApplication.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, UserManager<ApplicationUserEntity> userManager) =>
        {

        });

        group.MapPost("/login", async (LoginRequest request, SignInManager<ApplicationUserEntity> signInManager) =>
        {

        });

        group.MapGet("/google-login", (HttpContext context) =>
        {

        });

        group.MapGet("/google-callback", (HttpContext context) =>
        {

        });
    }
}

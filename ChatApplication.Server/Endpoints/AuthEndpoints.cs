using ChatApplication.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace ChatApplication.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapGet("/google-login", (HttpContext context) =>
        {

        });

        group.MapGet("/google-callback", (HttpContext context) =>
        {

        });
    }
}

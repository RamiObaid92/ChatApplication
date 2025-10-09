using ChatApplication.Server.Data.Entities;
using ChatApplication.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;

namespace ChatApplication.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapGet("/google-login", () =>
        {
            return Results.Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/auth/google-callback"
            },
            authenticationSchemes: ["Google"]);
        });

        group.MapGet("/google-callback", async (HttpContext context, SignInManager<ApplicationUserEntity> signInManager, UserManager<ApplicationUserEntity> userManager, ITokenService tokenService) =>
        {
            var authenticateResult = await context.AuthenticateAsync("Identity.External");
            if (!authenticateResult.Succeeded)
            {
                return Results.Unauthorized();
            }

            var providerKey = authenticateResult.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = authenticateResult.Principal?.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal?.FindFirstValue(ClaimTypes.Name);

            if (providerKey == null || email == null)
            {
                return Results.BadRequest("Error loading external login information.");
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync("Google", providerKey, isPersistent: false);

            ApplicationUserEntity? user;

            if (signInResult.Succeeded)
            {
                user = await userManager.FindByLoginAsync("Google", providerKey);
            }
            else
            {
                user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUserEntity
                    {
                        UserName = email,
                        Email = email,
                        DisplayName = name
                    };

                    var createResult = await userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        return Results.BadRequest(createResult.Errors);
                    }
                }

                var addLoginResult = await userManager.AddLoginAsync(user, new UserLoginInfo("Google", providerKey, "Google"));

                if (!addLoginResult.Succeeded)
                {
                    return Results.BadRequest(addLoginResult.Errors);
                }
            }

            if (user == null)
            {
                return Results.BadRequest("Error loading user information.");
            }

            var jwt = tokenService.GenerateJwtToken(user);
            return Results.Redirect($"https://your-frontend-app.com/login-callback?token={jwt}");
        });
    }
}

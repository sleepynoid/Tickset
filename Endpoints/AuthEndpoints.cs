using System.Security.Claims;
using Tickset.Data;
using Tickset.DTOs;
using Tickset.Services;

namespace Tickset.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", async (CreateUserDto dto, AuthService auth, TokenService token) =>
        {
            var user = await auth.RegisterAsync(dto);
            if (user == null)
                return Results.Conflict(new { message = "Email already exists" });

            var accessToken = token.GenerateToken(user.Id, user.Email);
            return Results.Created($"/api/users/{user.Id}",
                new AuthResponseDto(accessToken, user.Id, user.Email));
        });

        app.MapPost("/api/auth/login", async (LoginDto dto, AuthService auth, TokenService token) =>
        {
            var user = await auth.LoginAsync(dto);
            if (user == null)
                return Results.Unauthorized();

            var accessToken = token.GenerateToken(user.Id, user.Email);
            return Results.Ok(new AuthResponseDto(accessToken, user.Id, user.Email));
        });

        app.MapPost("/api/auth/me", async (ClaimsPrincipal user, AuthService auth, TokenService token) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            return Results.Ok(new { userId, email });
        }).RequireAuthorization();
    }
}

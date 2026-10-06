using Tickset.Data;
using Tickset.DTOs;
using Tickset.Services;

namespace Tickset.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", async (CreateUserDto dto, AuthService auth) =>
        {
            var user = await auth.RegisterAsync(dto);
            if (user == null)
                return Results.Conflict(new { message = "Email already exists" });

            return Results.Created($"/api/users/{user.Id}", new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            });
        });

        app.MapPost("/api/auth/login", async (LoginDto dto, AuthService auth) =>
        {
            var user = await auth.LoginAsync(dto);
            if (user == null)
                return Results.Unauthorized();

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            });
        });
    }
}
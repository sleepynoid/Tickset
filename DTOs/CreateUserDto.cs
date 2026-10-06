namespace Tickset.DTOs;

public record CreateUserDto(
    string Email,
    string FirstName,
    string LastName,
    string Password
);
public record AuthResponseDto(
    string Token,
    Guid UserId,
    string Email
);
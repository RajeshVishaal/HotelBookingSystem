using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Dto;

public record RegisterRequest(
    [Required] [MinLength(1)] string FirstName,
    [Required] [MinLength(1)] string LastName,
    [Required] [EmailAddress] string EmailAddress,
    [Required] [MinLength(6)] string Password
);

public record LoginRequest(
    [Required] [EmailAddress] string EmailAddress,
    [Required] string Password
);

public record AuthResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string EmailAddress,
    string? Message
);
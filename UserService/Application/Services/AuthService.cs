using Common.Dto;
using UserService.Application.Dto;
using UserService.Application.Services.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data.Repositories;

namespace UserService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public AuthService(IUserRepository userRepository, IUserService userService)
    {
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest Dto)
    {
        var existing = await _userRepository.GetByEmailAsync(Dto.EmailAddress);
        if (existing != null)
            return ApiResponse<AuthResponse>.Fail("Email address already registered.");
        var password = Dto.Password;
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var hashedPwd = BCrypt.Net.BCrypt.HashPassword(password);


        var user = new User
        {
            FirstName = Dto.FirstName,
            LastName = Dto.LastName,
            EmailAddress = Dto.EmailAddress,
            PasswordHash = hashedPwd
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        var response = new AuthResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.EmailAddress,
            "User registered successfully"
        );

        return ApiResponse<AuthResponse>.Ok(response, "User registered successfully");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest Dto)
    {
        var user = await _userRepository.GetByEmailAsync(Dto.EmailAddress);
        if (user == null)
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        if (!_userService.VerifyPassword(Dto.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        var response = new AuthResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.EmailAddress,
            "Login successful"
        );

        return ApiResponse<AuthResponse>.Ok(response, "Login successful");
    }
}
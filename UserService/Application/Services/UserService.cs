using Common.Dto;
using UserService.Application.Dto;
using UserService.Application.Services.Interfaces;
using UserService.Infrastructure.Data.Repositories;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<AuthResponse>> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse<AuthResponse>.Fail($"User with ID {id} not found.");

        var Dto = new AuthResponse(user.Id, user.FirstName, user.LastName, user.EmailAddress, "User found");
        return ApiResponse<AuthResponse>.Ok(Dto, "User retrieved successfully");
    }

    public async Task<ApiResponse<AuthResponse>> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return ApiResponse<AuthResponse>.Fail($"User with email '{email}' not found.");

        var Dto = new AuthResponse(user.Id, user.FirstName, user.LastName, user.EmailAddress, "User found");
        return ApiResponse<AuthResponse>.Ok(Dto, "User retrieved successfully");
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
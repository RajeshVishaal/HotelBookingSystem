using Common.Dto;
using UserService.Application.Dto;

namespace UserService.Application.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<AuthResponse>> GetUserByIdAsync(Guid id);
    Task<ApiResponse<AuthResponse>> GetUserByEmailAsync(string email);
    bool VerifyPassword(string password, string passwordHash);
}
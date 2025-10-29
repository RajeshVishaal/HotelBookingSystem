using Common.Dto;
using UserService.Application.Dto;

namespace UserService.Application.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest Dto);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest Dto);
}
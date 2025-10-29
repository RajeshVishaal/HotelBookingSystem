using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Dto;
using UserService.Application.Services.Interfaces;

namespace UserService.Controllers;

/// <summary>
///     Provides endpoints for registering and authenticating users.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <param name="Dto">
    ///     User registration details including first name, last name, email, and password.
    /// </param>
    /// <returns>
    ///     A success message and user profile information upon successful registration.
    /// </returns>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Email already exists or input validation failed.</response>
    /// <example>
    ///     POST /api/v1/Auth/signup
    ///     {
    ///     "firstName": "John",
    ///     "lastName": "Doe",
    ///     "email": "john.doe@example.com",
    ///     "password": "StrongPass@123"
    ///     }
    /// </example>
    [HttpPost("signup")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest Dto)
    {
        var result = await _authService.RegisterAsync(Dto);
        return result.Success
            ? CreatedAtAction(nameof(Register), ApiResponse<AuthResponse>.Ok(result.Data, result.Message))
            : BadRequest(ApiResponse<object>.Fail(result.Message));
    }

    /// <summary>
    ///     Logs a user into the system and issues an authentication token.
    /// </summary>
    /// <param name="Dto">
    ///     User login credentials including email and password.
    /// </param>
    /// <returns>
    ///     Returns user details and an access token upon successful login.
    /// </returns>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid email or password.</response>
    /// <example>
    ///     POST /api/v1/Auth/login
    ///     {
    ///     "email": "vishaal@gmail.com",
    ///     "password": "StrongPassword@123"
    ///     }
    /// </example>
    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest Dto)
    {
        var result = await _authService.LoginAsync(Dto);
        return result.Success
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Data, result.Message))
            : Unauthorized(ApiResponse<object>.Fail(result.Message));
    }
}
using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Dto;
using UserService.Application.Services.Interfaces;

namespace UserService.Controllers;

/// <summary>
///     Provides endpoints for managing user profiles and retrieving user information.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    ///     Gets user profile details by their ID.
    /// </summary>
    /// <param name="id">The user’s GUID.</param>
    /// <returns>Profile information including name, email, and user ID.</returns>
    /// <response code="200">User found and returned successfully.</response>
    /// <response code="404">No user found for the specified ID.</response>
    /// <remarks>
    ///     Returns key profile fields such as:
    ///     - User ID
    ///     - First and last name
    ///     - Email address
    /// </remarks>
    /// <example>
    ///     GET /api/v1/users/123e4567-e89b-12d3-a456-426614174000
    /// </example>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result.Success
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Data, result.Message))
            : NotFound(ApiResponse<object>.Fail(result.Message));
    }

    /// <summary>
    ///     Gets user profile details by their email address.
    /// </summary>
    /// <param name="email">The user’s email address (case-insensitive).</param>
    /// <returns>Profile information including user ID, full name, and email.</returns>
    /// <response code="200">User found and returned successfully.</response>
    /// <response code="404">No user found with the specified email address.</response>
    /// <response code="400">Email parameter is missing or invalid.</response>
    /// <example>
    ///     GET /api/v1/users/by-email?email=vishaal@gmail.com
    /// </example>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new Common.Exceptions.ValidationException("Email address cannot be empty.",
                new { Parameter = "email" });

        var result = await _userService.GetUserByEmailAsync(email);
        return result.Success
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Data, result.Message))
            : NotFound(ApiResponse<object>.Fail(result.Message));
    }
}
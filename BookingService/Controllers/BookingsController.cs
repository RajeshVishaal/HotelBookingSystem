using BookingService.Application.Dto;
using BookingService.Application.Exceptions;
using BookingService.Application.Services;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

/// <summary>
///     Provides endpoints for creating and retrieving hotel room bookings.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Tags("Bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    ///     Gets the details of a booking using its unique reference code.
    /// </summary>
    /// <param name="bookingReference">
    ///     The unique booking reference (for example, <c>BK-01K8F6837EAHETZ</c>).
    /// </param>
    /// <returns>
    ///     Returns the booking information if the reference is valid.
    /// </returns>
    /// <response code="200">Booking details found and returned successfully.</response>
    /// <response code="404">No booking found for the given reference.</response>
    [HttpGet("ref/{bookingReference}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingDetails([FromRoute] string bookingReference)
    {
        var result = await _bookingService.GetBookingByReferenceAsync(bookingReference);
        return Ok(ApiResponse<BookingDetailsResponse>.Ok(result, "Booking details retrieved successfully."));
    }

    /// <summary>
    ///     Creates a new hotel room booking.
    /// </summary>
    /// <param name="idempotencyKey">
    ///     A unique request identifier used to prevent duplicate bookings
    ///     (passed in the <c>Idempotency-Key</c> header).
    /// </param>
    /// <param name="request">
    ///     The booking request containing hotel, user, date range, and room details.
    /// </param>
    /// <returns>
    ///     Returns a booking confirmation along with the generated booking reference.
    /// </returns>
    /// <response code="200">Booking created successfully.</response>
    /// <response code="400">Invalid request or no rooms available.</response>
    /// <response code="409">A duplicate booking request was detected.</response>
    /// <response code="500">An unexpected error occurred while processing the booking.</response>
    [HttpPost("reserve")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<ReservationReceipt>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReserveRoom(
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        [FromBody] ReservationRequest request)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new Common.Exceptions.ValidationException(
                "Missing Idempotency-Key header. Please provide a unique key to prevent duplicate submissions.",
                new { Header = "Idempotency-Key" });

        var result = await _bookingService.ReserveRoomAsync(request, idempotencyKey);
        return Ok(ApiResponse<ReservationReceipt>.Ok(result, "Booking created successfully."));
    }
}
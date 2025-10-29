using Common.Dto;
using InventoryService.Application.Dto;
using InventoryService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

/// <summary>
///     Provides endpoints for managing hotels — including searching, checking room availability,
///     and retrieving detailed hotel information.
/// </summary>
[Tags("Hotels")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelSearchService;
    private readonly IRoomAvailabilityService _roomAvailabilityService;

    public HotelsController(IHotelService hotelSearchService, IRoomAvailabilityService roomAvailabilityService)
    {
        _hotelSearchService = hotelSearchService;
        _roomAvailabilityService = roomAvailabilityService;
    }

    /// <summary>
    ///     Searches for hotels by name, location, date range, and guest count.
    /// </summary>
    /// <param name="name">Hotel name or city to search for (optional).</param>
    /// <param name="checkIn">Optional check-in date (YYYY-MM-DD).</param>
    /// <param name="checkOut">Optional check-out date (YYYY-MM-DD).</param>
    /// <param name="guests">Optional number of guests.</param>
    /// <param name="page">Page number for paginated results (default: 1).</param>
    /// <param name="pageSize">Number of results per page (default: 10, maximum: 100).</param>
    /// <returns>A paginated list of hotels that match the provided search filters.</returns>
    /// <response code="200">Search completed successfully.</response>
    /// <remarks>
    ///     You can combine filters like city, date range, and guest count for more accurate results.
    /// </remarks>
    /// <example>
    ///     GET /api/v1/Hotels/search?name=London&amp;checkIn=2025-12-01&amp;checkOut=2025-12-05&amp;guests=2&amp;page=1&amp;
    ///     pageSize=10
    /// </example>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<HotelSearchResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchHotels(
        [FromQuery] string? name,
        [FromQuery] DateOnly? checkIn,
        [FromQuery] DateOnly? checkOut,
        [FromQuery] int? guests,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _hotelSearchService.SearchAsync(name, checkIn, checkOut, guests, page, pageSize);
        return Ok(ApiResponse<PaginatedResponse<HotelSearchResult>>.Ok(result, "Search completed successfully."));
    }

    /// <summary>
    /// Reserves available rooms in a specific hotel for the given date range.
    /// </summary>
    /// <param name="req">Reservation details, including hotel ID, stay dates, and room preferences.</param>
    /// <returns>Reservation confirmation along with booking status.</returns>
    /// <response code="200">Rooms reserved successfully.</response>
    /// <response code="409">No rooms available for the requested dates or room type.</response>
    /// <response code="400">Invalid request payload.</response>
    /// <remarks>
    /// This endpoint is primarily used internally by the Booking Service.
    /// It ensures availability checks and room reservations are handled atomically.
    /// </remarks>
    [HttpPost("reserve")]
    [ProducesResponseType(typeof(ApiResponse<RoomReservationReceipt>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reserve([FromBody] RoomReservationRequest req)
    {
        var receipt = await _hotelSearchService.ReserveRoomsAsync(req);
        return Ok(ApiResponse<RoomReservationReceipt>.Ok(receipt, "Rooms reserved successfully."));
    }


    /// <summary>
    ///     Generates a booking summary that includes pricing, taxes, and room details
    ///     before finalizing the reservation.
    /// </summary>
    /// <param name="req">Request payload containing hotel ID, selected rooms, and date range.</param>
    /// <returns>Comprehensive summary including rate breakdown and room information.</returns>
    /// <response code="200">Booking summary generated successfully.</response>
    /// <response code="400">Invalid input or hotel not found.</response>
    [HttpPost("booking-summary")]
    [ProducesResponseType(typeof(ApiResponse<BookingSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingSummary([FromBody] BookingSummaryRequest req)
    {
        var summary = await _hotelSearchService.GetBookingSummaryAsync(req);
        return Ok(ApiResponse<BookingSummaryResponse>.Ok(summary, "Booking summary generated successfully."));
    }

    /// <summary>
    ///     Retrieves a high level summary of a hotel including its key details and ratings.
    /// </summary>
    /// <param name="hotelId">GUID of the hotel.</param>
    /// <returns>Basic hotel information such as name, location, and overall rating.</returns>
    /// <response code="200">Hotel information returned successfully.</response>
    /// <response code="404">No hotel found with the provided ID.</response>
    /// <example>
    ///     GET /api/v1/Hotels/123e4567-e89b-12d3-a456-426614174000/info
    /// </example>
    [HttpGet("{hotelId}/info")]
    [ProducesResponseType(typeof(ApiResponse<HotelSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHotelSummary(Guid hotelId)
    {
        var hotel = await _hotelSearchService.GetHotelSummaryAsync(hotelId);
        return Ok(ApiResponse<HotelSummary>.Ok(hotel, "Hotel summary retrieved successfully."));
    }


    /// <summary>
    ///     Checks room availability for a given hotel, date range, and number of guests.
    /// </summary>
    /// <param name="hotelId">GUID of the hotel.</param>
    /// <param name="request">Request object specifying stay dates and guest count.</param>
    /// <returns>List of available room categories with pricing and occupancy details.</returns>
    /// <response code="200">Availability retrieved successfully.</response>
    /// <response code="400">Invalid request or hotel ID mismatch.</response>
    /// <remarks>
    ///     Use this endpoint before making a reservation to confirm room availability.
    /// </remarks>
    [HttpPost("{hotelId}/rooms/check-availability")]
    [ProducesResponseType(typeof(ApiResponse<AvailabilityCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckAvailability(Guid hotelId, [FromBody] AvailabilityCheckRequest request)
    {
        if (hotelId != request.HotelId)
            throw new Common.Exceptions.ValidationException("HotelId mismatch between URL and request body.",
                new { UrlHotelId = hotelId, BodyHotelId = request.HotelId });

        var response = await _roomAvailabilityService.CheckAvailabilityAsync(request);
        return Ok(ApiResponse<AvailabilityCheckResponse>.Ok(response, "Availability retrieved successfully."));
    }


    /// <summary>
    ///     Retrieves detailed information about a hotel — including its rooms, facilities, and live pricing.
    /// </summary>
    /// <param name="hotelId">The hotel’s unique identifier (GUID).</param>
    /// <param name="checkIn">Optional check-in date (YYYY-MM-DD).</param>
    /// <param name="checkOut">Optional check-out date (YYYY-MM-DD).</param>
    /// <param name="guests">Optional number of guests.</param>
    /// <returns>Complete hotel details with available rooms and price information.</returns>
    /// <response code="200">Hotel details returned successfully.</response>
    /// <response code="404">No hotel found with the provided ID or no available rooms.</response>
    /// <example>
    ///     GET /api/v1/Hotels/123e4567-e89b-12d3-a456-426614174000?checkIn=2025-12-01&amp;checkOut=2025-12-05&amp;guests=2
    /// </example>
    [HttpGet("{hotelId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<HotelDetails>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHotelDetails(
        Guid hotelId,
        [FromQuery] DateOnly? checkIn,
        [FromQuery] DateOnly? checkOut,
        [FromQuery] int? guests)
    {
        var result = await _hotelSearchService.GetHotelDetailsAsync(hotelId, checkIn, checkOut, guests);
        return Ok(ApiResponse<HotelDetails>.Ok(result, "Hotel details retrieved successfully."));
    }
}
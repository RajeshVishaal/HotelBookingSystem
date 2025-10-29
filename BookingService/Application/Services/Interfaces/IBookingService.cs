using BookingService.Application.Dto;

namespace BookingService.Application.Services;

public interface IBookingService
{
    Task<BookingDetailsResponse> ReserveRoomAsync(ReservationRequest req, string idempotencyKey);
    Task<BookingDetailsResponse> GetBookingByReferenceAsync(string bookingReference);
    Task<List<BookingDetailsResponse>> GetBookingsByUserIdAsync(Guid userId);
}
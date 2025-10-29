using BookingService.Application.Dto;

namespace BookingService.Application.Services;

public interface IBookingService
{
    Task<ReservationReceipt> ReserveRoomAsync(ReservationRequest req, string idempotencyKey);
    Task<BookingDetailsResponse> GetBookingByReferenceAsync(string bookingReference);
}
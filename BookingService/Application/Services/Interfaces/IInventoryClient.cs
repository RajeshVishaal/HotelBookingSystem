using BookingService.Application.Dto;
using Common.Dto;

namespace BookingService.Application.Services.Interfaces;

public interface IInventoryClient
{
    Task<ReservationReceipt> ReserveRoomAsync(ReservationRequest body);
    Task<HotelSummary?> GetHotelSummaryAsync(Guid hotelId);
}
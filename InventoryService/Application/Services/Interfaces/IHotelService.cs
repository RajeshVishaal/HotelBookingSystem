using Common.Dto;
using InventoryService.Application.Dto;

namespace InventoryService.Application.Services.Interfaces;

public interface IHotelService
{
    Task<HotelSummary?> GetHotelSummaryAsync(Guid hotelId);

    Task<BookingSummaryResponse> GetBookingSummaryAsync(
        BookingSummaryRequest req
    );

    Task<RoomReservationReceipt> ReserveRoomsAsync(RoomReservationRequest req);

    Task<PaginatedResponse<HotelSearchResult>?> SearchAsync(
        string? name,
        DateOnly? checkIn,
        DateOnly? checkOut,
        int? guests,
        int page,
        int pageSizet);

    Task<HotelDetails> GetHotelDetailsAsync(
        Guid hotelId,
        DateOnly? checkIn,
        DateOnly? checkOut,
        int? guests);
}
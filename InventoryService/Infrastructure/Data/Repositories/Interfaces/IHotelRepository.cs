using Common.Dto;
using Common.Interfaces;
using InventoryService.Application.Dto;
using InventoryService.Domain.Entities;

namespace InventoryService.Infrastructure.Data.Repositories.Interfaces;

public interface IHotelRepository : IBaseRepository<Hotel>
{
    new Task<Hotel?> GetByIdAsync(Guid hotelId);

    Task<RoomReservationReceipt> ReserveRoomsAsync(
        Guid hotelId,
        List<RoomReservationItem> rooms,
        DateOnly checkIn,
        DateOnly checkOut
    );

    Task<BookingSummaryResponse> GetBookingSummaryAsync(
        Guid hotelId,
        List<RoomReservationItem> rooms,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests
    );

    Task<PaginatedResponse<HotelSearchResult>?> SearchHotelsAsync(
        string? name,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? guests,
        int skip,
        int take);

    Task<HotelDetails?> GetHotelDetailsAsync(
        Guid hotelId,
        DateOnly? fromDate,
        DateOnly? toDate,
        int? guests);
}
using InventoryService.Application.Dto;

namespace InventoryService.Infrastructure.Data.Repositories.Interfaces;

public interface IRoomAvailabilityRepository
{
    Task<bool> CheckAvailabilityAsync(
        Guid hotelId,
        List<Guid> roomCategoryIds,
        DateOnly checkIn,
        DateOnly checkOut);
}
using InventoryService.Application.Dto;
using InventoryService.Application.Services.Interfaces;
using InventoryService.Infrastructure.Data.Repositories.Interfaces;

namespace InventoryService.Application.Services;

public class RoomAvailabilityService : IRoomAvailabilityService
{
    private readonly IRoomAvailabilityRepository _roomAvailabilityRepository;

    public RoomAvailabilityService(IRoomAvailabilityRepository roomAvailabilityRepository)
    {
        _roomAvailabilityRepository = roomAvailabilityRepository;
    }

    public async Task<AvailabilityCheckResponse> CheckAvailabilityAsync(AvailabilityCheckRequest request)
    {
        var available = await _roomAvailabilityRepository.CheckAvailabilityAsync(
            request.HotelId,
            request.RoomCategoryIds,
            request.CheckIn,
            request.CheckOut);

        return new AvailabilityCheckResponse
        {
            Available = available
        };
    }
}
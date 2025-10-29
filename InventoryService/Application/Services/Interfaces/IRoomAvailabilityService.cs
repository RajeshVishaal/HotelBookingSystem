using InventoryService.Application.Dto;

namespace InventoryService.Application.Services.Interfaces;

public interface IRoomAvailabilityService
{
    Task<AvailabilityCheckResponse> CheckAvailabilityAsync(AvailabilityCheckRequest request);
}
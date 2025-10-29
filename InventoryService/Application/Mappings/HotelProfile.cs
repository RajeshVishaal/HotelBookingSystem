using AutoMapper;
using InventoryService.Application.Dto;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Mappings;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelSummary>();
    }
}
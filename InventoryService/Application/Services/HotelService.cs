using AutoMapper;
using Common.Dto;
using InventoryService.Application.Dto;
using InventoryService.Application.Services.Interfaces;
using InventoryService.Infrastructure.Data.Repositories.Interfaces;

namespace InventoryService.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ILogger<HotelService> _logger;
    private readonly IMapper _mapper;

    public HotelService(
        IMapper mapper,
        IHotelRepository hotelRepository,
        ILogger<HotelService> logger)
    {
        _mapper = mapper;
        _hotelRepository = hotelRepository;
        _logger = logger;
    }

    public async Task<BookingSummaryResponse> GetBookingSummaryAsync(BookingSummaryRequest req)
    {
        if (req == null) throw new ArgumentNullException(nameof(req));

        var result = await _hotelRepository.GetBookingSummaryAsync(
            req.HotelId, req.Rooms, req.CheckIn, req.CheckOut, req.Guests);

        return result ?? throw new InvalidOperationException("Booking summary not found.");
    }

    public async Task<HotelSummary?> GetHotelSummaryAsync(Guid hotelId)
    {
        if (hotelId == Guid.Empty)
            throw new ArgumentException("HotelId cannot be empty.", nameof(hotelId));

        var hotel = await _hotelRepository.GetByIdAsync(hotelId);
        return hotel == null
            ? null
            : new HotelSummary
            {
                HotelId = hotel.Id,
                Id = hotel.Id,
                Name = hotel.Name,
                AddressLine = hotel.AddressLine,
                City = hotel.City,
                Country = hotel.Country,
                AverageRating = hotel.AverageRating,
                Comment = hotel.Comment,
                ImageUrl = hotel.ImageUrl
            };
    }

    public async Task<RoomReservationReceipt> ReserveRoomsAsync(RoomReservationRequest req)
    {
        return await _hotelRepository.ReserveRoomsAsync(
            req.HotelId, req.Rooms, req.CheckIn, req.CheckOut);
    }

    public async Task<PaginatedResponse<HotelSearchResult>?> SearchAsync(string? name, DateOnly? checkIn,
        DateOnly? checkOut,
        int? guests, int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = Math.Min(pageSize <= 0 ? 10 : pageSize, 100);
        var skip = (page - 1) * pageSize;

        var result =
            await _hotelRepository.SearchHotelsAsync(name, checkIn, checkOut, guests, skip, pageSize);
        return result ?? EmptyResponse();
    }

    public async Task<HotelDetails> GetHotelDetailsAsync(Guid hotelId, DateOnly? checkIn, DateOnly? checkOut,
        int? guests)
    {
        if (hotelId == Guid.Empty)
            throw new ArgumentException("HotelId cannot be empty.", nameof(hotelId));

        var details = await _hotelRepository.GetHotelDetailsAsync(hotelId, checkIn, checkOut, guests);
        return details ?? throw new InvalidOperationException("Hotel details not found.");
    }

    private static PaginatedResponse<HotelSearchResult> EmptyResponse()
    {
        return new PaginatedResponse<HotelSearchResult>
        {
            PageNo = 0,
            TotalRecords = 0,
            PageSize = 0,
            Items = new List<HotelSearchResult>(),
            HasNext = false,
            HasPrevious = false
        };
    }
}
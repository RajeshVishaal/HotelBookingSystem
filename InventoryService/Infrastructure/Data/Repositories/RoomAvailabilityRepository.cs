using InventoryService.Application.Dto;
using InventoryService.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Repositories.Interfaces;

public class RoomAvailabilityRepository : IRoomAvailabilityRepository
{
    private readonly AppDbContext _db;

    public RoomAvailabilityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BookingSummaryResponse> GetBookingSummaryAsync(
        Guid hotelId,
        List<RoomReservationItem> rooms,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests
    )
    {
        var totalNights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
        if (totalNights <= 0)
            throw new InvalidOperationException("Invalid date range.");

        var categoryIds = rooms.Select(r => r.RoomCategoryId).Distinct().ToList();

        var hotel = await _db.Hotels
                        .AsNoTracking()
                        .Where(h => h.Id == hotelId)
                        .Select(h => new HotelSummary
                        {
                            HotelId = h.Id,
                            Id = h.Id,
                            Name = h.Name,
                            City = h.City,
                            Country = h.Country,
                            ImageUrl = h.ImageUrl
                        })
                        .FirstOrDefaultAsync()
                    ?? throw new NoAvailabilityException($"Hotel with ID {hotelId} not found.");

        var roomData = await (
            from rc in _db.RoomCategories.AsNoTracking()
            join rt in _db.RoomTypes.AsNoTracking() on rc.RoomTypeId equals rt.Id
            join rp in _db.RoomPricings.AsNoTracking() on rc.Id equals rp.RoomCategoryId
            where rc.HotelId == hotelId && categoryIds.Contains(rc.Id)
            select new
            {
                rc.Id,
                RoomTypeName = rt.Name,
                rp.BaseRate,
                rc.ImageUrls,
                Facilities = rc.RoomFacilities.Select(f => f.Name).ToList()
            }
        ).ToListAsync();

        var roomSummaries = new List<BookingRoomSummary>();
        var totalCost = 0m;

        foreach (var selected in rooms)
        {
            var data = roomData.FirstOrDefault(r => r.Id == selected.RoomCategoryId)
                       ?? throw new InvalidOperationException($"Room category {selected.RoomCategoryId} not found.");

            var subtotal = data.BaseRate * selected.Quantity * totalNights;
            totalCost += subtotal;

            roomSummaries.Add(new BookingRoomSummary
            {
                RoomCategoryId = selected.RoomCategoryId,
                RoomTypeName = data.RoomTypeName,
                Quantity = selected.Quantity,
                PricePerNight = data.BaseRate,
                Subtotal = subtotal,
                Images = data.ImageUrls ?? new List<string>(),
                Facilities = data.Facilities
            });
        }

        var summary = new BookingSummary
        {
            HotelId = hotelId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalNights = totalNights,
            Guests = guests,
            Rooms = roomSummaries,
            TotalCost = totalCost
        };

        var response = new BookingSummaryResponse
        {
            Hotel = hotel,
            Summary = summary
        };

        return response;
    }

    public async Task<bool> CheckAvailabilityAsync(
        Guid hotelId,
        List<Guid> roomCategoryIds,
        DateOnly checkIn,
        DateOnly checkOut)
    {
        var totalNights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
        if (totalNights <= 0) return false;

        var availableRoomCategories = await (
            from ra in _db.RoomAvailabilities.AsNoTracking()
            where ra.HotelId == hotelId
                  && roomCategoryIds.Contains(ra.RoomCategoryId)
                  && ra.Date >= checkIn
                  && ra.Date < checkOut
                  && ra.TotalCount - ra.TotalBooked > 0
            group ra by ra.RoomCategoryId
            into g
            where g.Select(x => x.Date).Distinct().Count() == totalNights
            select g.Key
        ).ToListAsync();

        return availableRoomCategories.Count == roomCategoryIds.Count;
    }
}
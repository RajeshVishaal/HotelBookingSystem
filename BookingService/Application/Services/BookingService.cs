using BookingService.Application.Dto;
using BookingService.Application.Exceptions;
using BookingService.Application.Services.Interfaces;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Common.Exceptions;

namespace BookingService.Application.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;
    private readonly IInventoryClient _inventory;
    private readonly IBookingRepository _repo;

    public BookingService(
        IBookingRepository repo,
        IInventoryClient inventory,
        AppDbContext db)
    {
        _repo = repo;
        _inventory = inventory;
        _db = db;
    }

    public async Task<BookingDetailsResponse> ReserveRoomAsync(
        ReservationRequest request,
        string idempotencyKey)
    {
        var existing = await _repo.GetByIdempotencyKeyAsync(idempotencyKey);
        if (existing is not null)
            throw new IdempotentRequestException(existing.BookingReference);

        var reservation = await _inventory.ReserveRoomAsync(request);
        if (reservation is null || reservation is null)
            throw new BookingValidationException("Failed to reserve rooms.");

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingReference = GenerateReference(),
                IdempotencyKey = idempotencyKey,
                HotelId = reservation.HotelId,
                UserId = request.UserId,
                CheckIn = reservation.CheckIn,
                CheckOut = reservation.CheckOut,
                Guests = request.Guests,
                TotalCost = reservation.TotalCost
            };

            foreach (var room in reservation.Rooms)
                booking.Rooms.Add(new BookingRoomInfo
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    RoomCategoryId = room.RoomCategoryId,
                    Quantity = room.Quantity,
                    BaseRate = room.PricePerNight,
                    Subtotal = room.Subtotal
                });

            await _repo.AddAsync(booking);
            await _repo.SaveChangesAsync();
            await tx.CommitAsync();

            var hotel = await _inventory.GetHotelSummaryAsync(booking.HotelId);

            return new BookingDetailsResponse
                {
                    BookingReference = booking.BookingReference,
                    HotelId = booking.HotelId,
                    HotelName = hotel?.Name ?? "",
                    HotelImageUrl = hotel?.ImageUrl ?? "",
                    UserId = booking.UserId,
                    CheckIn = booking.CheckIn,
                    CheckOut = booking.CheckOut,
                    Guests = booking.Guests,
                    TotalCost = booking.TotalCost,
                    CreatedAt = booking.CreatedAt,
                    Rooms = booking.Rooms.Select(r => new ReservedRoom
                    {
                        RoomCategoryId = r.RoomCategoryId,
                        Quantity = r.Quantity,
                        BaseRate = r.BaseRate,
                        Subtotal = r.Subtotal
                    }).ToList()
                };
            
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            await tx.RollbackAsync();

            var existingBooking = await _repo.GetByIdempotencyKeyAsync(idempotencyKey);
            if (existingBooking is not null)
                throw new IdempotentRequestException(existingBooking.BookingReference);

            throw;
        }
    }

    public async Task<BookingDetailsResponse> GetBookingByReferenceAsync(
        string bookingReference)
    {
        var booking = await _repo.GetByReferenceAsync(bookingReference);
        if (booking is null)
            throw new BookingNotFoundException(bookingReference);

        var hotel = await _inventory.GetHotelSummaryAsync(booking.HotelId);
        if (hotel is null)
            throw new ExternalServiceException($"Unable to retrieve hotel info for ID {booking.HotelId}.");

        return new BookingDetailsResponse
        {
            BookingReference = booking.BookingReference,
            HotelId = booking.HotelId,
            HotelName = hotel.Name,
            HotelImageUrl = hotel.ImageUrl,
            UserId = booking.UserId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Guests = booking.Guests,
            TotalCost = booking.TotalCost,
            CreatedAt = booking.CreatedAt,
            Rooms = booking.Rooms.Select(r => new ReservedRoom
            {
                RoomCategoryId = r.RoomCategoryId,
                Quantity = r.Quantity,
                BaseRate = r.BaseRate,
                Subtotal = r.Subtotal
            }).ToList()
        };
    }

    public async Task<List<BookingDetailsResponse>> GetBookingsByUserIdAsync(Guid userId)
    {
        var bookings = await _repo.GetBookingsByUserIdAsync(userId);
        
        if (!bookings.Any())
            return new List<BookingDetailsResponse>();

        var hotelIds = bookings.Select(b => b.HotelId).Distinct().ToList();
        var hotels = new Dictionary<Guid, HotelSummary>();

        foreach (var hotelId in hotelIds)
        {
            var hotel = await _inventory.GetHotelSummaryAsync(hotelId);
            if (hotel != null)
                hotels[hotelId] = hotel;
        }

        return bookings.Select(booking => new BookingDetailsResponse
        {
            BookingReference = booking.BookingReference,
            HotelId = booking.HotelId,
            HotelName = hotels.ContainsKey(booking.HotelId) ? hotels[booking.HotelId].Name : "Unknown Hotel",
            HotelImageUrl = hotels.ContainsKey(booking.HotelId) ? hotels[booking.HotelId].ImageUrl : "",
            UserId = booking.UserId,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Guests = booking.Guests,
            TotalCost = booking.TotalCost,
            CreatedAt = booking.CreatedAt,
            Rooms = booking.Rooms.Select(r => new ReservedRoom
            {
                RoomCategoryId = r.RoomCategoryId,
                Quantity = r.Quantity,
                BaseRate = r.BaseRate,
                Subtotal = r.Subtotal
            }).ToList()
        }).ToList();
    }

    private static string GenerateReference()
    {
        return $"BK-{Ulid.NewUlid():N}".ToUpperInvariant()[..15];
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
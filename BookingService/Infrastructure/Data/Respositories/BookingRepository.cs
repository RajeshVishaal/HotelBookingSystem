using BookingService.Domain.Entities;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Domain.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _db;

    public BookingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Booking?> GetByReferenceAsync(string bookingReference)
    {
        return await _db.Bookings
            .Include(b => b.Rooms)
            .FirstOrDefaultAsync(b => b.BookingReference == bookingReference);
    }


    public async Task<Booking?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _db.Bookings
            .Include(b => b.Rooms)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.IdempotencyKey == idempotencyKey);
    }


    public async Task AddAsync(Booking booking)
    {
        await _db.Bookings.AddAsync(booking);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
using BookingService.Domain.Entities;

namespace BookingService.Infrastructure.Data.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<Booking?> GetByReferenceAsync(string bookingReference);
    Task<List<Booking>> GetBookingsByUserIdAsync(Guid userId);

    Task AddAsync(Booking booking);
    Task SaveChangesAsync();
}
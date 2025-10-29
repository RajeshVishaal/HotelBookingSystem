using Common.Exceptions;

namespace BookingService.Application.Exceptions;

public class BookingNotFoundException : NotFoundException
{
    public string BookingReference { get; }

    public BookingNotFoundException(string bookingReference)
        : base($"Booking with reference '{bookingReference}' was not found.",
            new { BookingReference = bookingReference })
    {
        BookingReference = bookingReference;
    }
}
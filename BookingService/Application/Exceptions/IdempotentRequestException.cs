using Common.Exceptions;

namespace BookingService.Application.Exceptions;

public class IdempotentRequestException : ConflictException
{
    public string BookingReference { get; }

    public IdempotentRequestException(string bookingReference)
        : base($"Duplicate booking request detected. Existing booking reference: {bookingReference}",
            new { BookingReference = bookingReference, Reused = true })
    {
        BookingReference = bookingReference;
    }
}
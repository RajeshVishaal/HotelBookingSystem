using Common.Exceptions;

namespace BookingService.Application.Exceptions;

public class BookingValidationException : ValidationException
{
    public BookingValidationException(string message, object? errorDetails = null)
        : base(message, errorDetails)
    {
    }
}
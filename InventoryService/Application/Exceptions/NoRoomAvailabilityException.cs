using Common.Exceptions;

namespace InventoryService.Application.Exceptions;

public class NoAvailabilityException : NotFoundException
{
    public NoAvailabilityException(string message, object? errorDetails = null)
        : base(message, errorDetails)
    {
    }
}
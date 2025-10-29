using System.Net;

namespace Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message, object? errorDetails = null)
        : base(message, HttpStatusCode.NotFound, errorDetails)
    {
    }
}
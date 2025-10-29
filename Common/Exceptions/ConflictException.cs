using System.Net;

namespace Common.Exceptions;

public class ConflictException : BaseException
{
    public ConflictException(string message, object? errorDetails = null)
        : base(message, HttpStatusCode.Conflict, errorDetails)
    {
    }
}
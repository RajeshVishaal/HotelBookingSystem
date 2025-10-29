using System.Net;

namespace Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Authentication failed.", object? errorDetails = null)
        : base(message, HttpStatusCode.Unauthorized, errorDetails)
    {
    }
}
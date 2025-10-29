using System.Net;

namespace Common.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message, object? errorDetails = null)
        : base(message, HttpStatusCode.BadRequest, errorDetails)
    {
    }
}
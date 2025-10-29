using System.Net;

namespace Common.Exceptions;

public abstract class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public object? ErrorDetails { get; }

    protected BaseException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        object? errorDetails = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorDetails = errorDetails;
    }
}
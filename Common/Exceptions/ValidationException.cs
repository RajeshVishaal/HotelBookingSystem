using System.Net;

namespace Common.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(string message, object? errorDetails = null)
        : base(message, HttpStatusCode.BadRequest, errorDetails)
    {
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", HttpStatusCode.BadRequest, new { Errors = errors })
    {
    }
}
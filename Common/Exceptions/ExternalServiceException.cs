using System.Net;

namespace Common.Exceptions;

public class ExternalServiceException : BaseException
{
    public ExternalServiceException(string message, object? errorDetails = null, Exception? innerException = null)
        : base(message, HttpStatusCode.ServiceUnavailable, errorDetails, innerException)
    {
    }

    public ExternalServiceException(string serviceName, string operation, Exception? innerException = null)
        : base($"Failed to communicate with {serviceName} service during {operation}.",
            HttpStatusCode.ServiceUnavailable,
            new { ServiceName = serviceName, Operation = operation },
            innerException)
    {
    }
}
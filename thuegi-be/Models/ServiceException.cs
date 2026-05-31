namespace thuegi_be.Models;

public sealed class ServiceException : Exception
{
    public int StatusCode { get; }

    public ServiceException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

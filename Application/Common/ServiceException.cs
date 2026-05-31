namespace Application.Common;

public sealed class ServiceException : Exception
{
    public int StatusCode { get; }

    public ServiceException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

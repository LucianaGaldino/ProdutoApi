using ProdutoAPI.Enums;

namespace ProdutoAPI.Exceptions;

public class AppException : Exception
{
    public StatusCode StatusCode { get; }

    public AppException(string message, StatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

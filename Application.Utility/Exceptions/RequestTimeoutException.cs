namespace Application.Utility.Exceptions;

public class RequestTimeoutException : Exception
{
    public RequestTimeoutException(string message) : base(message)
    {

    }
}
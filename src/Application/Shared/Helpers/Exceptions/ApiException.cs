namespace Application.Shared.Helpers.Exceptions;

public abstract class ApiException : Exception
{
    public int StatusCode { get; }
    public IReadOnlyList<string> Errors { get; }

    protected ApiException(string message, int statusCode, IEnumerable<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = (errors ?? Array.Empty<string>()).ToList();
    }
}

public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message = "Not found.")
        : base(message, 404)
    {
    }
}

public sealed class BadRequestException : ApiException
{
    public BadRequestException(string message = "Bad request.", IEnumerable<string>? errors = null)
        : base(message, 400, errors)
    {
    }
}
namespace Application.Shared.Helpers.Responses;

public class BaseResponse
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];

    public static BaseResponse Ok(string message = "OK", int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };

    public static BaseResponse Fail(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList() ?? []
        };
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public static BaseResponse<T> Ok(T data, string message = "OK", int statusCode = 200)
        => new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

    public static BaseResponse<T> Fail(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        => new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList() ?? [],
            Data = default
        };
}
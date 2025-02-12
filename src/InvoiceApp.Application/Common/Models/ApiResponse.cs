using System.Net;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }

    // Static factory methods for common responses
    public static ApiResponse Success(string message = "Operation successful")
    {
        return new ApiResponse
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = message
        };
    }

    public static ApiResponse Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = new List<string> { message }
        };
    }

}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    // Static factory methods for generic responses
    public static ApiResponse<T> Success(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = message,
            Data = data
        };
    }

    public static new ApiResponse<T> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = new List<string> { message },
            Data = default
        };
    }
}

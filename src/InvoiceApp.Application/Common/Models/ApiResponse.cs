// Application/Common/ApiResponse.cs
public class ApiResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResponse<T> Success<T>(T? data, string message = "")
        => new() { Status = true, Data = data, Message = message };

    public static ApiResponse<object> Failure(string message)
        => new() { Status = false, Message = message };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}
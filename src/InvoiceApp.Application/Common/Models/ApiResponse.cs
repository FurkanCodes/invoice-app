using System.Net;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }


}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

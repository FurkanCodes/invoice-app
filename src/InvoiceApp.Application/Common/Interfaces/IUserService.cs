using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public interface IUserService
{
  Guid UserId { get; }
}

public class UserService : IUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ILogger<UserService> _logger;

  public UserService(IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
  {
    _httpContextAccessor = httpContextAccessor;
    _logger = logger;
  }

  public Guid UserId
  {
    get
    {
      var user = _httpContextAccessor.HttpContext?.User;

      if (user == null)
      {
        _logger.LogError("HttpContext.User is null");
        throw new UnauthorizedAccessException("User context not found");
      }

      // Try different ways to find the claim
      var userIdClaim = user.FindFirst("uid")?.Value ??
                      user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                      user.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
      _logger.LogInformation("User ID claim: {UserIdClaim}", userIdClaim);
      _logger.LogInformation("Claims found in token: {Claims}",
          string.Join(", ", user.Claims.Select(c => $"{c.Type}: {c.Value}")));

      if (string.IsNullOrEmpty(userIdClaim))
      {
        _logger.LogError("User ID claim not found in token");
        throw new UnauthorizedAccessException("User ID not found in token");
      }

      if (!Guid.TryParse(userIdClaim, out Guid userId))
      {
        _logger.LogError("Failed to parse user ID: {UserIdClaim}", userIdClaim);
        throw new UnauthorizedAccessException("Invalid user ID format");
      }

      return userId;
    }
  }
}

using System;
using System.Security.Claims;
using InvoiceApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InvoiceApp.Infrastructure.Services;

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

  public Guid UserId
  {
    get
    {
      var userIdClaim = _httpContextAccessor.HttpContext?.User?
          .FindFirstValue(ClaimTypes.NameIdentifier);

      if (Guid.TryParse(userIdClaim, out var userId))
        return userId;

      throw new UnauthorizedAccessException("User is not authenticated.");
    }
  }

  public string Email => _httpContextAccessor.HttpContext?.User?
      .FindFirstValue(ClaimTypes.Email) ?? string.Empty;
}

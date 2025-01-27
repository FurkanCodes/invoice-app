// Infrastructure/Services/AuthService.cs
using System.Security.Cryptography;
using System.Text;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
public class AuthService(AppDbContext context, ITokenService tokenService, IHttpContextAccessor httpContextAccessor) : IAuthService
{

  public async Task<AuthResponseDto> Register(UserRegisterDto userDto)
  {
    // Validate email uniqueness
    if (await context.Users.AnyAsync(u => u.Email == userDto.Email))
      throw new ArgumentException("Email is already registered");

    // Hash password
    using var hmac = new HMACSHA512();
    var user = new User
    {
      Email = userDto.Email,
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
      PasswordSalt = hmac.Key,
      CreatedAt = DateTime.UtcNow
    };

    // Save to database
    context.Users.Add(user);
    await context.SaveChangesAsync();

    // Generate tokens
    try
    {
      var (token, expiration) = tokenService.GenerateAccessToken(user);
      var refreshToken = tokenService.GenerateRefreshToken();

      // Save refresh token
      await context.RefreshTokens.AddAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await context.SaveChangesAsync();

      // Set refresh token cookie
      httpContextAccessor.HttpContext?.Response.Cookies.Append(
          "refreshToken",
          refreshToken,
          new CookieOptions
          {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7),
            SameSite = SameSiteMode.Strict
          });

      return new AuthResponseDto(true, token, expiration, refreshToken);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"TOKEN GENERATION ERROR: {ex}");
      return new AuthResponseDto(true, null, DateTime.UtcNow, null);

    }
  }
  public async Task<AuthResponseDto> Login(UserLoginDto userDto)
  {
    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email) ?? throw new UnauthorizedAccessException("Invalid credentials");

    // Verify password
    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));

    if (!computedHash.SequenceEqual(user.PasswordHash))
      throw new UnauthorizedAccessException("Invalid credentials");
    var (accessToken, accessExpiration) = tokenService.GenerateAccessToken(user);
    var refreshToken = tokenService.GenerateRefreshToken();

    await context.RefreshTokens.AddAsync(new RefreshToken
    {
      Token = refreshToken,
      UserId = user.Id,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      User = user  // Add this line
    });
    await context.SaveChangesAsync();



    httpContextAccessor.HttpContext?.Response.Cookies.Append(
           "refreshToken",
           refreshToken,
           new CookieOptions
           {
             HttpOnly = true,
             Secure = true,  // For HTTPS only
             Expires = DateTime.UtcNow.AddDays(7),
             SameSite = SameSiteMode.Strict
           });

    return new AuthResponseDto(true, accessToken, accessExpiration, refreshToken);

  }
}
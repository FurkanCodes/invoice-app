using System.Security.Cryptography;
using System.Text;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FluentEmail.Core;
using System.Net;

public class AuthService(AppDbContext context, ITokenService tokenService, IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
  public async Task<ApiResponse<AuthResponseDto>> Register(UserRegisterDto userDto)
  {
    try
    {
      if (await context.Users.AnyAsync(u => u.Email == userDto.Email))
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Conflict,
          Message = "Email is already registered",
          Errors = new List<string> { "EMAIL_EXISTS" }
        };
      }

      using var hmac = new HMACSHA512();
      var user = new User
      {
        Email = userDto.Email,
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
        PasswordSalt = hmac.Key,
        CreatedAt = DateTime.UtcNow,
        IsEmailVerified = false
      };

      context.Users.Add(user);
      await context.SaveChangesAsync();

      var (token, expiration) = tokenService.GenerateAccessToken(user);
      var refreshToken = tokenService.GenerateRefreshToken();
      var verificationCode = GenerateVerificationCode();

      await context.RefreshTokens.AddAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await context.SaveChangesAsync();

      // Send verification email through email service
      var emailResult = await emailService.SendVerificationEmail(user.Email, verificationCode);
      if (!emailResult.IsSuccess)
      {
        Console.WriteLine($"Email failed: {emailResult.Message}");
      }

      SetRefreshTokenCookie(refreshToken);

      return new ApiResponse<AuthResponseDto>
      {
        IsSuccess = true,
        StatusCode = HttpStatusCode.Created,
        Message = "Registration successful",
        Data = new AuthResponseDto
        {
          Token = token,
          Expiration = expiration,
          RefreshToken = refreshToken
        }
      };
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Registration error: {ex}");
      return new ApiResponse<AuthResponseDto>
      {
        IsSuccess = false,
        StatusCode = HttpStatusCode.InternalServerError,
        Message = "Registration failed",
        Errors = new List<string> { "SERVER_ERROR" }
      };
    }
  }

  public async Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto userDto)
  {
    try
    {
      var user = await context.Users
          .FirstOrDefaultAsync(u => u.Email == userDto.Email);

      if (user == null)
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Unauthorized,
          Message = "Invalid credentials",
          Errors = new List<string> { "INVALID_CREDENTIALS" }
        };
      }

      using var hmac = new HMACSHA512(user.PasswordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));

      if (!computedHash.SequenceEqual(user.PasswordHash))
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Unauthorized,
          Message = "Invalid credentials",
          Errors = new List<string> { "INVALID_CREDENTIALS" }
        };
      }

      var (accessToken, accessExpiration) = tokenService.GenerateAccessToken(user);
      var refreshToken = tokenService.GenerateRefreshToken();

      await context.RefreshTokens.AddAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await context.SaveChangesAsync();

      SetRefreshTokenCookie(refreshToken);

      return new ApiResponse<AuthResponseDto>
      {
        IsSuccess = true,
        StatusCode = HttpStatusCode.OK,
        Message = "Login successful",
        Data = new AuthResponseDto
        {
          Token = accessToken,
          Expiration = accessExpiration,
          RefreshToken = refreshToken
        }
      };
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Login error: {ex}");
      return new ApiResponse<AuthResponseDto>
      {
        IsSuccess = false,
        StatusCode = HttpStatusCode.InternalServerError,
        Message = "Login failed",
        Errors = new List<string> { "SERVER_ERROR" }
      };
    }
  }

  private void SetRefreshTokenCookie(string refreshToken)
  {
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
  }

  private static string GenerateVerificationCode()
  {
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    return new string(Enumerable.Repeat(chars, 6)
        .Select(s => s[random.Next(s.Length)]).ToArray());
  }
}

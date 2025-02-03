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
using InvoiceApp.Application.Common.Interfaces;
public class AuthService(
    IAuthRepository authRepository,
    ITokenService tokenService,
    IEmailService emailService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
  private readonly IAuthRepository _authRepository = authRepository;
  private readonly ITokenService _tokenService = tokenService;
  private readonly IEmailService _emailService = emailService;
  private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

  public async Task<ApiResponse<AuthResponseDto>> Register(UserRegisterDto userDto)
  {
    try
    {
      if (await _authRepository.EmailExistsAsync(userDto.Email))
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Conflict,
          Message = "Email is already registered",
          Errors = ["EMAIL_EXISTS"]
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

      await _authRepository.AddUserAsync(user);
      await _authRepository.SaveChangesAsync();

      var (token, expiration) = _tokenService.GenerateAccessToken(user);
      var refreshToken = _tokenService.GenerateRefreshToken();
      var verificationCode = GenerateVerificationCode();

      await _authRepository.AddRefreshTokenAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await _authRepository.SaveChangesAsync();

      var emailResult = await _emailService.SendVerificationEmail(user.Email);
      if (!emailResult.IsSuccess)
      {
        Console.WriteLine($"Email failed: {emailResult.Message}");
      }

      SetRefreshTokenCookie(refreshToken);

      return new ApiResponse<AuthResponseDto>
      {
        IsSuccess = true,
        StatusCode = HttpStatusCode.Created,
        Message = "Registration successful. Please check your email.",
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
        Errors = ["SERVER_ERROR"]
      };
    }
  }

  public async Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto userDto)
  {
    try
    {
      var user = await _authRepository.GetUserByEmailAsync(userDto.Email);

      if (user == null)
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Unauthorized,
          Message = "Invalid credentials",
          Errors = ["INVALID_CREDENTIALS"]
        };
      }

      if (!user.IsEmailVerified)
      {
        return new ApiResponse<AuthResponseDto>
        {
          IsSuccess = false,
          StatusCode = HttpStatusCode.Unauthorized,
          Message = "Email is not verified",
          Errors = ["EMAIL_VERIFICATION_ERROR"]
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
          Errors = ["INVALID_CREDENTIALS"]
        };
      }

      var (accessToken, accessExpiration) = _tokenService.GenerateAccessToken(user);
      var refreshToken = _tokenService.GenerateRefreshToken();

      await _authRepository.AddRefreshTokenAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await _authRepository.SaveChangesAsync();

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
        Errors = ["SERVER_ERROR"]
      };
    }
  }

  private void SetRefreshTokenCookie(string refreshToken)
  {
    _httpContextAccessor.HttpContext?.Response.Cookies.Append(
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
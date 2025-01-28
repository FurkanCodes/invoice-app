// Infrastructure/Services/AuthService.cs
using System.Security.Cryptography;
using System.Text;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FluentEmail.Core;
public class AuthService(AppDbContext context, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IFluentEmail fluentEmail) : IAuthService
{

  public async Task<ApiResponse<AuthResponseDto>> Register(UserRegisterDto userDto)
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
      CreatedAt = DateTime.UtcNow,
      IsEmailVerified = false
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

      // try
      // {
      //   var emailResponse = await fluentEmail
      //       .To(user.Email)
      //       .Subject("Verify Your Email")
      //       .Body("The body")
      //       .SendAsync();

      //   if (!emailResponse.Successful)
      //   {
      //     // Log the error
      //     Console.WriteLine($"Failed to send email: {emailResponse.ErrorMessages}");
      //   }
      }
      catch (Exception ex)
      {
        // Log the exception
        Console.WriteLine($"Email sending failed: {ex}");
      }
      return ApiResponse.Success(
       new AuthResponseDto
       {

       },
                     "Registration successful. Check your inbox to confirm your email address."
                 );

    }
    catch (Exception ex)
    {
      Console.WriteLine($"TOKEN GENERATION ERROR: {ex}");
      throw;
    }
  }
  public async Task<ApiResponse<AuthResponseDto>> Login(UserLoginDto userDto)
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
    return ApiResponse.Success(
                     new AuthResponseDto
                     {
                       Token = accessToken,
                       Expiration = accessExpiration,
                       RefreshToken = refreshToken
                     },
                     "Login successful"
                 );


  }
}
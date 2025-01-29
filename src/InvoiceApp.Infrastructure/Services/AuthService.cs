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
    Console.WriteLine("Starting registration process...");

    // Validate email uniqueness
    if (await context.Users.AnyAsync(u => u.Email == userDto.Email))
    {
      Console.WriteLine("Email already exists");
      throw new ArgumentException("Email is already registered");
    }

    Console.WriteLine("Email is unique, proceeding...");

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

    Console.WriteLine("User object created, saving to database...");

    // Save to database
    context.Users.Add(user);
    await context.SaveChangesAsync();

    Console.WriteLine("User saved to database, generating tokens...");

    try
    {
      var (token, expiration) = tokenService.GenerateAccessToken(user);
      var refreshToken = tokenService.GenerateRefreshToken();

      Console.WriteLine("Tokens generated, saving refresh token...");

      // Save refresh token
      await context.RefreshTokens.AddAsync(new RefreshToken
      {
        Token = refreshToken,
        UserId = user.Id,
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        User = user
      });
      await context.SaveChangesAsync();

      Console.WriteLine("Starting email send...");

      try
      {
        var emailResponse = await fluentEmail
            .To(user.Email)
            .Subject("Welcome to InvoiceApp - Verify Your Email")
            .Body($@"
        <h2>Welcome to InvoiceApp!</h2>
        <p>Thank you for registering. Please verify your email address.</p>
        <p>Your verification code is: THERE IS NONE YET!</p>
    ")
            .SendAsync();

        Console.WriteLine(emailResponse.Successful
            ? "Email sent successfully"
            : $"Email failed: {string.Join(", ", emailResponse.ErrorMessages)}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Email error: {ex.Message}");
        // Continue with registration even if email fails
      }

      Console.WriteLine("Completing registration...");

      return ApiResponse.Success(
          new AuthResponseDto
          {
            Token = token,
            Expiration = expiration,
            RefreshToken = refreshToken
          },
          "Registration successful"
      );
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error during registration: {ex.Message}");
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
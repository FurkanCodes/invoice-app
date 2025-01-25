// Infrastructure/Services/AuthService.cs
using System.Security.Cryptography;
using System.Text;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AuthService(AppDbContext context, ITokenService tokenService) : IAuthService
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

    // Generate JWT
    var (token, expiration) = tokenService.GenerateToken(user);
    return new AuthResponseDto(token, expiration);
  }
  public async Task<AuthResponseDto> Login(UserLoginDto userDto)
  {
    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
    if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

    // Verify password
    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));

    if (!computedHash.SequenceEqual(user.PasswordHash))
      throw new UnauthorizedAccessException("Invalid credentials");
    var (token, expiration) = tokenService.GenerateToken(user);
    return new AuthResponseDto(token, expiration);
  }
}
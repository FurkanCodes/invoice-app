// Infrastructure/Services/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InvoiceApp.Infrastructure.Services;



public class TokenService(IConfiguration config) : ITokenService
{
  public (string token, DateTime expiration) GenerateToken(User user)
  {
    var secretKey = config["JwtSettings:Secret"];


    var claims = new List<Claim>
        {
            new("uid", user.Id.ToString()),
            new("email", user.Email),
            new("name", user.Email.Split('@')[0])

        };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT secret key is not configured"))
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var expiration = DateTime.UtcNow.AddMinutes(
        config.GetValue<int>("JwtSettings:ExpirationMinutes")
    );

    var token = new JwtSecurityToken(
        issuer: config["JwtSettings:Issuer"],
        audience: config["JwtSettings:Audience"],
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expiration,
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return (token: tokenString, expiration);
  }
}


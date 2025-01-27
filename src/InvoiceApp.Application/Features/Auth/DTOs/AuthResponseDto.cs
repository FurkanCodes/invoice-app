namespace InvoiceApp.Application.Features.Auth.DTOs;

public class AuthResponseDto(string token, DateTime expiration, string refreshToken)
{
  public string Token { get; set; } = token;
  public string? RefreshToken { get; set; } = refreshToken;
  public DateTime Expiration { get; set; } = expiration;
}

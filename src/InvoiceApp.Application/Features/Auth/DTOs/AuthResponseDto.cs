namespace InvoiceApp.Application.Features.Auth.DTOs;

public class AuthResponseDto(string token, DateTime expiration)
{
  public string Token { get; set; } = token;
  public DateTime Expiration { get; set; } = expiration;
}

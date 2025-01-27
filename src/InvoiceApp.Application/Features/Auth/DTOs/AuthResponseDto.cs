namespace InvoiceApp.Application.Features.Auth.DTOs;

public class AuthResponseDto(bool Status, string token, DateTime expiration, string refreshToken)
{
  public bool? Status { get; set; } = Status;
  public string? Token { get; set; } = token;
  public string? RefreshToken { get; set; } = refreshToken;
  public DateTime? Expiration { get; set; } = expiration;
}

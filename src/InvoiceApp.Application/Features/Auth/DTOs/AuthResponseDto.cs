namespace InvoiceApp.Application.Features.Auth.DTOs;

// Application/Features/Auth/DTOs/AuthResponseDto.cs
public class AuthResponseDto
{
  public string Token { get; set; }
  public DateTime Expiration { get; set; }
  public string RefreshToken { get; set; }

  public AuthResponseDto(string token, DateTime expiration, string refreshToken)
  {
    Token = token;
    Expiration = expiration;
    RefreshToken = refreshToken;
  }

  public AuthResponseDto()
  {


  } // Add parameterless constructor
}

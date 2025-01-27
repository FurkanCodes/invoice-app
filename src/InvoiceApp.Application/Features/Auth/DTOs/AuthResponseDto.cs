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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
  public AuthResponseDto()
  {


  } // Add parameterless constructor
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

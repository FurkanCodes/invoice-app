using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Interfaces
{
  public interface ITokenService
  {
    (string Token, DateTime Expiration) GenerateAccessToken(User user);
    string GenerateRefreshToken();
  }
}

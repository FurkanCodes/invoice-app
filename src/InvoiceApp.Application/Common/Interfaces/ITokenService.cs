using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Interfaces;
public interface ITokenService
{
  (string token, DateTime expiration) GenerateToken(User user);
}

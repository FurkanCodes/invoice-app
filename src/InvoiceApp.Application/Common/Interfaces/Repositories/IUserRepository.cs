using InvoiceApp.Domain.Entities;

public interface IUserRepository
{
  Task<User?> GetByIdAsync(Guid userId);
    Task<bool> IsEmailConfirmedAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task<EmailVerification?> GetLatestEmailVerificationAsync(string email);
    Task<EmailVerification?> GetEmailVerificationByTokenAsync(string tokenHash);
}
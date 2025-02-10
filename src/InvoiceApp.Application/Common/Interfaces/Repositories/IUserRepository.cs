using InvoiceApp.Domain.Entities;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<bool> IsEmailConfirmedAsync(Guid userId);
}

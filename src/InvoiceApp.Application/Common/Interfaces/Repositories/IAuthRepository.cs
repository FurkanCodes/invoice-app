using InvoiceApp.Domain.Entities;
namespace InvoiceApp.Application.Common.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<RefreshToken> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);

        Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);


    }
}
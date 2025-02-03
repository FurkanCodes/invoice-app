using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Token == token &&
                i.IsValid &&
                i.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        token.IsValid = false;
        await UpdateRefreshTokenAsync(token, cancellationToken);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        _context.RefreshTokens.Update(token);
    }
}
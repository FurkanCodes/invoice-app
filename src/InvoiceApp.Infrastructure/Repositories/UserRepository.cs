using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;

namespace InvoiceApp.Infrastructure.Repositories
{
  public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);   
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.IsEmailVerified ?? false;
    }

    public async Task<EmailVerification?> GetLatestEmailVerificationAsync(string email)
    {
        return await _context.EmailVerifications
            .Include(ev => ev.User)
            .Where(ev => ev.User!.Email == email)
            .OrderByDescending(ev => ev.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<EmailVerification?> GetEmailVerificationByTokenAsync(string tokenHash)
    {
        return await _context.EmailVerifications
            .Include(ev => ev.User)
            .FirstOrDefaultAsync(ev => 
                ev.VerificationTokenHash == tokenHash && 
                ev.ExpiresAt > DateTime.UtcNow &&
                ev.Status != EmailVerificationStatus.Success);
    }
}
}

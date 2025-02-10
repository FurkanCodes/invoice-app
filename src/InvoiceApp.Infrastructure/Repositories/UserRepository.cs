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

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> IsEmailConfirmedAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.IsEmailVerified ?? false;
        }
    }
}

using InvoiceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Application.Common.Interfaces
{
  public interface IApplicationDbContext
  {
    DbSet<Invoice> Invoices { get; set; }

    DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
  }
}
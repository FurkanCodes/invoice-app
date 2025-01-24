// 📂 Infrastructure/Persistence/AppDbContext.cs
using InvoiceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Invoice> Invoices { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Invoice>(entity =>
    {
        entity.HasKey(i => i.Id);
        entity.Property(i => i.ClientName)
            .IsRequired()
            .HasMaxLength(100);
            
        entity.Property(i => i.Amount)
            .HasColumnType("decimal(18,2)");
            
        entity.Property(i => i.DueDate)
            .IsRequired();

        // Add these new configurations 👇
        entity.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        entity.Property(i => i.DeletedAt)
            .IsRequired(false);
    });

    base.OnModelCreating(modelBuilder);
}
}
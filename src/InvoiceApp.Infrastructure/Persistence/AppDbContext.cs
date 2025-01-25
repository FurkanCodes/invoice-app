// 📂 Infrastructure/Persistence/AppDbContext.cs
using InvoiceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Application;
using InvoiceApp.Application.Common.Interfaces;

namespace InvoiceApp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext

{
    public DbSet<Invoice> Invoices { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

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


            entity.Property(i => i.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(i => i.DeletedAt)
                .IsRequired(false);

            entity.Property(i => i.TaxId)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(i => i.CompanyRegistration)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(i => i.LegalAddress)
        .IsRequired();
            entity.Property(i => i.Currency)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(i => i.TaxRate)
                .IsRequired();
            entity.Property(i => i.PaymentTerms)
                .IsRequired();
            entity.Property(i => i.InvoiceNumber)
                .IsRequired();
            entity.Property(i => i.IssueDate)
                .IsRequired();


        });

        base.OnModelCreating(modelBuilder);
    }
}


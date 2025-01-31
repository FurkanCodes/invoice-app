// 📂 Infrastructure/Persistence/AppDbContext.cs
using InvoiceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Application;
using InvoiceApp.Application.Common.Interfaces;

namespace InvoiceApp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext

{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<EmailVerification> EmailVerifications { get; set; }

    public DbSet<Customer> Customers { get; set; }

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

        modelBuilder.Entity<User>(entity =>
          {
              entity.HasKey(u => u.Id);

              entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(255);

              entity.Property(u => u.PasswordHash)
                  .IsRequired();

              entity.Property(u => u.PasswordSalt)
                  .IsRequired();

              entity.Property(u => u.CreatedAt)
                  .IsRequired()
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

              // Add unique index on email
              entity.HasIndex(u => u.Email)

                      .IsUnique();
              entity.Property(u => u.IsEmailVerified);
          });

        modelBuilder.Entity<Invoice>()
                    .HasOne(i => i.User)
                    .WithMany(u => u.Invoices)
                    .HasForeignKey(i => i.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

        // Add any additional configuration like indexes
        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .Property(i => i.Amount)
            .HasPrecision(18, 2);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefreshToken>(entity =>
 {
     entity.HasKey(rt => rt.Id);

     entity.Property(rt => rt.Token)
         .IsRequired()
         .HasMaxLength(255);

     entity.Property(rt => rt.CreatedAt)
         .IsRequired()
         .HasDefaultValueSql("CURRENT_TIMESTAMP");

     entity.Property(rt => rt.ExpiresAt)
         .IsRequired();

     entity.Property(rt => rt.IsValid)
         .IsRequired()
         .HasDefaultValue(true);

     entity.Property(rt => rt.UserId)
         .IsRequired();

     // Configure relationship with User
     entity.HasOne(rt => rt.User)
         .WithMany(u => u.RefreshTokens)
         .HasForeignKey(rt => rt.UserId)
         .OnDelete(DeleteBehavior.Cascade);

     // Add index on Token for faster lookups
     entity.HasIndex(rt => rt.Token)
         .IsUnique();
 });

        modelBuilder.Entity<EmailVerification>(entity =>
   {
       // Primary Key
       entity.HasKey(ev => ev.Id);

       // Properties
       entity.Property(e => e.VerificationTokenHash)
           .IsRequired()
           .HasMaxLength(128);

       entity.Property(e => e.VerificationCodeHash)
           .IsRequired()
           .HasMaxLength(128);

       entity.Property(e => e.ExpiresAt)
           .IsRequired();

       entity.Property(e => e.CreatedAt)
           .IsRequired();

       entity.Property(e => e.Status)
           .IsRequired()
           .HasConversion<string>();

       // Relationships
       entity.HasOne(ev => ev.User)
           .WithMany(u => u.EmailVerifications)
           .HasForeignKey(ev => ev.UserId)
           .OnDelete(DeleteBehavior.Cascade);

       // Indexes
       entity.HasIndex(ev => new { ev.UserId, ev.ExpiresAt });
   });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Configure relationships
            entity.HasOne(c => c.User)
                .WithMany(u => u.Customers)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure enums
            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Configure required fields
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);

            // Configure address fields
            entity.Property(e => e.StreetAddress).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);

            // Configure billing fields
            entity.Property(e => e.PaymentTerms).HasMaxLength(50);
            entity.Property(e => e.DefaultCurrency).HasMaxLength(3);
            entity.Property(e => e.TaxId).HasMaxLength(50);
            entity.Property(e => e.AccountNumber).HasMaxLength(50);

            // Configure audit fields
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("NOW()");

            // Add indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Email);

            // Configure table name and columns
            entity.ToTable("Customers");
            entity.Property(e => e.Id).HasColumnName("CustomerId");

        });

    }
}


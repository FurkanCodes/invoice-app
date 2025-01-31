using System.ComponentModel.DataAnnotations;


namespace InvoiceApp.Domain.Entities;
public class Customer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }  // Foreign key

    // Customer Type (Required)
    public CustomerType Type { get; set; }  // Enum: Organization/Person

    // Name Fields
    public string? OrganizationName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    // Contact Info
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Website { get; set; }

    // Address
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // Billing
    public string? PaymentTerms { get; set; }
    public string? DefaultCurrency { get; set; }
    public string? TaxId { get; set; }
    public string? AccountNumber { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    // Relationships
    public User User { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = [];
}

public enum CustomerType
{
    Organization,
    Person
}

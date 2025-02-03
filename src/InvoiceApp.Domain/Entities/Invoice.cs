using System.ComponentModel.DataAnnotations;
using InvoiceApp.Domain.Exceptions;

namespace InvoiceApp.Domain.Entities;

public class Invoice
{
    public Guid Id { get; private set; }
    public string ClientName { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DueDate { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Required]
    [StringLength(20)]
    public string TaxId { get; private set; }  // e.g., VAT Number

    [Required]
    [StringLength(50)]
    public string CompanyRegistration { get; private set; }

    [Required]
    public string LegalAddress { get; private set; }

    [Required]
    [StringLength(10)]
    public string Currency { get; private set; } = "USD";

    [Required]
    public decimal TaxRate { get; private set; }  // e.g., 0.20 for 20% VAT

    public decimal TaxAmount => Amount * TaxRate;
    public decimal TotalAmount => Amount + TaxAmount;

    [Required]
    public string PaymentTerms { get; private set; } = "Net 30 Days";

    [Required]
    public string InvoiceNumber { get; private set; }

    [Required]
    public DateTime IssueDate { get; private set; }
    public InvoiceStatus Status { get; set; }
    // Foreign key
    public Guid UserId { get; set; }

    // Make the navigation property nullable
    public User? User { get; set; }

    public Guid? CustomerId { get; private set; }
    public Customer Customer { get; set; } = null!;



    private Invoice()
    {
        ClientName = string.Empty;
        TaxId = string.Empty;
        CompanyRegistration = string.Empty;
        LegalAddress = string.Empty;
        InvoiceNumber = string.Empty;
    }

    public Invoice(
         Guid userId,
         string clientName,
         decimal amount,
         DateTime dueDate,
         string taxId,
         string companyRegistration,
         string legalAddress,
         string currency,
         decimal taxRate,
         string paymentTerms,
         string invoiceNumber,
         DateTime issueDate,
         Guid? customerId = null)
    {
        if (string.IsNullOrEmpty(clientName)) throw new DomainException("Client name is required.");
        if (amount <= 0) throw new DomainException("Amount must be positive.");
        if (string.IsNullOrEmpty(taxId)) throw new DomainException("Tax ID is required.");
        if (string.IsNullOrEmpty(companyRegistration)) throw new DomainException("Company registration is required.");
        if (string.IsNullOrEmpty(legalAddress)) throw new DomainException("Legal address is required.");
        if (string.IsNullOrEmpty(invoiceNumber)) throw new DomainException("Invoice number is required.");


        Id = Guid.NewGuid();

        ClientName = clientName;
        UserId = userId;
        Amount = amount;
        CustomerId = customerId;
        DueDate = dueDate;

        TaxId = taxId;

        CompanyRegistration = companyRegistration;

        LegalAddress = legalAddress;

        Currency = currency;

        TaxRate = taxRate;

        PaymentTerms = paymentTerms;

        InvoiceNumber = invoiceNumber;
        Status = InvoiceStatus.Draft;
        IssueDate = issueDate;
    }
}

public enum InvoiceStatus
{
    Draft,
    Pending,
    Paid,
    Overdue,
    Cancelled
}
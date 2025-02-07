using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Queries;

public class InvoiceDto
{
  // --- Basic Information ---

  /// <summary>
  /// Unique invoice identifier.
  /// </summary>
  /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
  public Guid Id { get; set; }

  /// <summary>
  /// Sequential invoice number (often user-friendly).
  /// </summary>
  /// <example>INV-2024-0001</example>
  public string InvoiceNumber { get; set; } = string.Empty;

  /// <summary>
  /// Date the invoice was issued.
  /// </summary>
  /// <example>2024-03-15T00:00:00</example>
  public DateTime IssueDate { get; set; }

  /// <summary>
  /// Due date for payment.
  /// </summary>
  /// <example>2024-03-31T00:00:00</example>
  public DateTime DueDate { get; set; }

  // --- Client Information ---

  /// <summary>
  /// Name of the client.
  /// </summary>
  /// <example>Microsoft Corporation</example>
  public string ClientName { get; set; } = string.Empty;
  /// <summary>
  /// Unique customer identifier.
  /// </summary>
  public Guid CustomerId { get; set; }

  //Consider adding a nested DTO for client if needed.
  // public CustomerSummaryDto Client { get; set; }


  // --- Amounts and Currency ---

  /// <summary>
  /// Currency of the invoice.
  /// </summary>
  /// <example>USD</example>
  public string Currency { get; set; } = string.Empty;  // Consider using a dedicated Currency type/enum

  /// <summary>
  /// The total amount of the invoice *before* taxes.
  /// </summary>
  /// <example>1800.00</example>
  public decimal Subtotal { get; set; }


  /// <summary>
  /// Total tax amount on the invoice.
  /// </summary>
  /// <example>199.99</example>
  public decimal TaxAmount { get; set; }


  /// <summary>
  /// The tax rate applied (e.g., 0.10 for 10%).
  /// </summary>
  /// <example>0.10</example>
  public decimal TaxRate { get; set; }

  /// <summary>
  /// Total amount due (including taxes, discounts, etc.).
  /// </summary>
  /// <example>1999.99</example>
  public decimal Amount { get; set; }  // Renamed from original Amount, keep TotalAmount name


  // --- Status and Payment Information ---
  /// <summary>
  /// Status of the invoice (Draft, Sent, Paid, Overdue, etc.).
  /// </summary>
  /// <example>Sent</example>
  public string Status { get; set; } = string.Empty; // Consider an enum (InvoiceStatus)
                                                     //public InvoiceStatus Status { get; set; } //Best practice


  /// <summary>
  /// Date the invoice was paid (if applicable).
  /// </summary>
  /// <example>2024-03-28T00:00:00</example>
  public DateTime? PaymentDate { get; set; } // Nullable, as it might not be paid yet

  /// <summary>
  /// Payment terms (e.g., "Net 30").
  /// </summary>
  /// <example>Net 30</example>
  public string PaymentTerms { get; set; } = string.Empty;

  /// <summary>
  /// Notes or comments related to payment (e.g., payment method).
  /// </summary>
  public string? PaymentNotes { get; set; }

  // --- Other Details ---

  /// <summary>
  /// A purchase order number associated with the invoice.
  /// </summary>
  /// <example>PO-12345</example>
  public string? PurchaseOrderNumber { get; set; }  // Optional

  /// <summary>
  /// Notes or comments on the invoice (for internal use or for the client).
  /// </summary>
  public string? Notes { get; set; }  // Optional

  /// <summary>
  /// Your company's legal address (for the invoice header).  Could be a nested object.
  /// </summary>
  public string? LegalAddress { get; set; }

  /// <summary>
  /// Unique identifier of the user who created the invoice.
  /// </summary>
  public Guid UserId { get; set; }


  // --- Soft Delete (Keep these) ---
  public DateTime? DeletedAt { get; set; }
  public bool IsDeleted { get; set; }

  /// <summary>
  /// List of items included in the invoice.
  /// </summary>
  public List<InvoiceItemDto> Items { get; set; } = new(); // Initialize to an empty list
}

// Create a separate DTO for invoice items:
public class InvoiceItemDto
{
  public Guid Id { get; set; }
  public string Description { get; set; } = string.Empty;
  public decimal Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Amount { get; set; } // Could be calculated: Quantity * UnitPrice
  public decimal? Discount { get; set; }

}
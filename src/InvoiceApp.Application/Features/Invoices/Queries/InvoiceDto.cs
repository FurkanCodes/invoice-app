using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Queries;

public class InvoiceDto
{

  /// <summary>
    /// Unique invoice identifier
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the client
    /// </summary>
    /// <example>Microsoft Corporation</example>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Invoice amount
    /// </summary>
    /// <example>1999.99</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// Due date for payment
    /// </summary>
    /// <example>2024-03-31T00:00:00</example>
    public DateTime DueDate { get; set; }
public DateTime? DeletedAt { get; set; }

public bool IsDeleted { get; set; }

}

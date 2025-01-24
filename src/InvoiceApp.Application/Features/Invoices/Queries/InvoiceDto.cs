using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Queries;

public class InvoiceDto
{

public Guid Id { get; set; }

public string ClientName { get; set; } = string.Empty;

public decimal Amount { get; set; }

public DateTime DueDate { get; set; }

public DateTime? DeletedAt { get; set; }

public bool IsDeleted { get; set; }

}

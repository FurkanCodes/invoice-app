using MediatR;
using System.ComponentModel.DataAnnotations;
using InvoiceApp.Application.Features.Invoices.Queries;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetDeletedInvoices;

public class GetDeletedInvoicesQuery : IRequest<PagedResponse<InvoiceDto>>
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1-100")]
    public int PageSize { get; set; } = 10;

    [DataType(DataType.DateTime)]
    public DateTime? DeletedAfter { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? DeletedBefore { get; set; }

    [StringLength(50, ErrorMessage = "Client name filter too long (max 50 chars)")]
    public string? ClientNameContains { get; set; }

    [AllowedValues("DeletedAt", "ClientName", "Amount", 
        ErrorMessage = "Invalid sort field")]
    public string SortBy { get; set; } = "DeletedAt";

    public bool SortDescending { get; set; } = true;
}
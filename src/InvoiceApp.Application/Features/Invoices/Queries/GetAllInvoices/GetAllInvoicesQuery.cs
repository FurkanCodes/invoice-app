// GetAllInvoicesQuery.cs
using MediatR;
using InvoiceApp.Application.Features.Invoices.Queries;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

public class GetAllInvoicesQuery : IRequest<PagedResponse<InvoiceDto>>
{
    public int PageNumber { get; set; } = 1;  // Default to first page
    public int PageSize { get; set; } = 10;   // Default page size
}
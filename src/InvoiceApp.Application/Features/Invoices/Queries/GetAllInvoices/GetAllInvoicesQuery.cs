// GetAllInvoicesQuery.cs
using MediatR;
using InvoiceApp.Application.Features.Invoices.Queries;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

public class GetAllInvoicesQuery : IRequest<PagedResponse<InvoiceDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
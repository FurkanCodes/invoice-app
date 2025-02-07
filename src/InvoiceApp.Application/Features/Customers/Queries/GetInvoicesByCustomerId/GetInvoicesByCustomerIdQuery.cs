// GetInvoicesByCustomerIdQuery.cs
using InvoiceApp.Application.Features.Invoices.Queries; // Ensure you have this using
using MediatR;

public record GetInvoicesByCustomerIdQuery : IRequest<PagedResponse<InvoiceDto>> // Correct return type
{
    public Guid CustomerId { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
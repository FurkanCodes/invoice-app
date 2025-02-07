// GetInvoicesByCustomerIdQuery.cs
using InvoiceApp.Application.Features.Invoices.Queries; // Ensure you have this using
using InvoiceApp.Domain.Entities;
using MediatR;

public record GetCustomerByIdQuery : IRequest<ApiResponse<Customer>> // Correct return type
{
    public Guid CustomerId { get; init; }

}
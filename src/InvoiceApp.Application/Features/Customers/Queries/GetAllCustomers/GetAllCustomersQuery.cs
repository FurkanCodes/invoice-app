// GetAllInvoicesQuery.cs
using MediatR;
using InvoiceApp.Application.Features.Invoices.Queries;
using System.ComponentModel.DataAnnotations;
using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllCustomers;

public class GetAllCustomersQuery : IRequest<PagedResponse<Customer>>
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 10")]
    public int PageSize { get; set; } = 10;

}
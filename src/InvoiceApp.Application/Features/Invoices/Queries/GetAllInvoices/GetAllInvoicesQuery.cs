// GetAllInvoicesQuery.cs
using MediatR;
using InvoiceApp.Application.Features.Invoices.Queries;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

public class GetAllInvoicesQuery : IRequest<PagedResponse<InvoiceDto>>
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int PageNumber { get; set; } = 1;
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 10")]
    public int PageSize { get; set; } = 10;

     [DataType(DataType.DateTime)]
    public DateTime? StartDate { get; set; }
     [DataType(DataType.DateTime)]
    public DateTime? EndDate { get; set; }
}
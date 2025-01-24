

using System.ComponentModel.DataAnnotations;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Domain.Entities;
using MediatR;

public class GetInvoiceByIdQuery : IRequest<InvoiceDto>
{

[Required (ErrorMessage = "Invoice Id is required")]
public Guid InvoiceId { get; set; }
}
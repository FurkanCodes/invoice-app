// DeleteInvoiceCommand.cs
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class DeleteInvoiceCommand : IRequest<Unit> // Unit = void in MediatR
{
    [Required(ErrorMessage = "Invoice ID is required")]
    public Guid InvoiceId { get; set; }
}
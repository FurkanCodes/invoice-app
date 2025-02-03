// DeleteInvoiceCommand.cs
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class DeleteCustomerCommand : IRequest<Unit> // Unit = void in MediatR
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }
}
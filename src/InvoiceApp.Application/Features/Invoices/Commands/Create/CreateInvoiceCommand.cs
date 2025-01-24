using MediatR;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class CreateInvoiceCommand : IRequest<Guid>
{
    public required string ClientName { get; set; }
    public required decimal Amount { get; set; }
    public required DateTime DueDate { get; set; }
}
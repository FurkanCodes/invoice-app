// DeleteInvoiceCommandHandler.cs
using MediatR;
using InvoiceApp.Domain.Exceptions;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class DeleteInvoiceCommandHandler(AppDbContext context)
    : IRequestHandler<DeleteInvoiceCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteInvoiceCommand command, 
        CancellationToken ct)
    {
  var invoice = context.Invoices.FirstOrDefaultAsync(i => i.Id == command.InvoiceId, ct) ?? throw new DomainException("Invoice not found");
  Console.WriteLine(invoice);
    return Unit.Value;
    }
}
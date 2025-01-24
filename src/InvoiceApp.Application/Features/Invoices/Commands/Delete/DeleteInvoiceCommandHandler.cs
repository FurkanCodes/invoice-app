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
        var invoice = await context.Invoices
            .FirstOrDefaultAsync(i => i.Id == command.InvoiceId, ct);

        if (invoice == null)
            throw new DomainException("Invoice not found");

        invoice.SoftDelete();
        await context.SaveChangesAsync(ct);

        return Unit.Value; // MediatR's "void" return
    }
}
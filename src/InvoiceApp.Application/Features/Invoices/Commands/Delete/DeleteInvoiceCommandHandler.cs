// DeleteInvoiceCommandHandler.cs
using MediatR;
using InvoiceApp.Domain.Exceptions;

using Microsoft.EntityFrameworkCore;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;

namespace InvoiceApp.Application.Features.Invoices.Commands;
public class DeleteInvoiceCommandHandler(IApplicationDbContext context)
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
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using MediatR;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class CreateInvoiceCommandHandler(AppDbContext context)
    : IRequestHandler<CreateInvoiceCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        // Create the invoice using domain logic
        var invoice = new Invoice(
            command.ClientName,
            command.Amount,
            command.DueDate);

        // Persist to PostgreSQL
        await context.Invoices.AddAsync(invoice, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}
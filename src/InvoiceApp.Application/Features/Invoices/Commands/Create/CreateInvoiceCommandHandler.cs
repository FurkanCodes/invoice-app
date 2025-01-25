using InvoiceApp.Application.Common.Interfaces;

using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Exceptions;

using MediatR;


namespace InvoiceApp.Application.Features.Invoices.Commands;

public class CreateInvoiceCommandHandler(IApplicationDbContext context, ICurrencyValidator currencyValidator)
    : IRequestHandler<CreateInvoiceCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {

        if (!currencyValidator.IsValidCode(command.Currency))
        {
            throw new DomainException("Invalid currency code");
        }

        var invoice = new Invoice(
            command.ClientName,
            command.Amount,
            command.DueDate,
            command.TaxId,
            command.CompanyRegistration,
            command.LegalAddress,
            command.Currency,
            command.TaxRate,
            command.PaymentTerms,
            command.InvoiceNumber,
            command.IssueDate);

        await context.Invoices.AddAsync(invoice, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return invoice.Id;
    }
}
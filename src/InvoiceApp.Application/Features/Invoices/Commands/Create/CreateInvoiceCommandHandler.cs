using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Exceptions;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace InvoiceApp.Application.Features.Invoices.Commands;
public class CreateInvoiceCommandHandler(
    IApplicationDbContext context,
    ICurrencyValidator currencyValidator,
    IUserService userService,
    ILogger<CreateInvoiceCommandHandler> logger) : IRequestHandler<CreateInvoiceCommand, Guid>
{


    public async Task<Guid> Handle(
      CreateInvoiceCommand command,
      CancellationToken cancellationToken)
    {
        try
        {
            var userId = userService.UserId;
            logger.LogInformation("Creating invoice for user: {UserId}", userId);

            if (!currencyValidator.IsValidCode(command.Currency))
            {
                throw new DomainException("Invalid currency code");
            }

            // Validate user exists
            var userExists = await context.Users
                .AnyAsync(u => u.Id == userId, cancellationToken);

            if (!userExists)
            {
                throw new DomainException($"User with ID {userId} not found");
            }

            // Check for duplicate invoice number
            var invoiceNumberExists = await context.Invoices
                .AnyAsync(i => i.InvoiceNumber == command.InvoiceNumber, cancellationToken);

            if (invoiceNumberExists)
            {
                throw new DomainException($"Invoice number '{command.InvoiceNumber}' already exists.");
            }

            if (command.CustomerId.HasValue)
            {
                var customerExists = await context.Customers
                    .AnyAsync(c => c.Id == command.CustomerId.Value, cancellationToken);
                if (!customerExists)
                {
                    throw new DomainException($"Customer with ID {command.CustomerId} not found.");
                }
            }

            // Create invoice with CustomerId
            var invoice = new Invoice(
                userId,
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
                command.IssueDate,
                command.CustomerId // Include CustomerId
            );


            await context.Invoices.AddAsync(invoice, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully created invoice {InvoiceId} for user {UserId}",
                invoice.Id, userId);

            return invoice.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating invoice: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}
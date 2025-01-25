using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Exceptions;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace InvoiceApp.Application.Features.Invoices.Commands;
public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrencyValidator _currencyValidator;
    private readonly IUserService _userService;
    private readonly ILogger<CreateInvoiceCommandHandler> _logger;

    public CreateInvoiceCommandHandler(
        IApplicationDbContext context,
        ICurrencyValidator currencyValidator,
        IUserService userService,
        ILogger<CreateInvoiceCommandHandler> logger)
    {
        _context = context;
        _currencyValidator = currencyValidator;
        _userService = userService;
        _logger = logger;
    }

    public async Task<Guid> Handle(
      CreateInvoiceCommand command,
      CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userService.UserId;
            _logger.LogInformation("Creating invoice for user: {UserId}", userId);

            if (!_currencyValidator.IsValidCode(command.Currency))
            {
                throw new DomainException("Invalid currency code");
            }

            // Validate user exists
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId, cancellationToken);

            if (!userExists)
            {
                throw new DomainException($"User with ID {userId} not found");
            }

            // Check for duplicate invoice number
            var invoiceNumberExists = await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == command.InvoiceNumber, cancellationToken);

            if (invoiceNumberExists)
            {
                throw new DomainException($"Invoice number '{command.InvoiceNumber}' already exists.");
            }

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
                command.IssueDate
            );

            await _context.Invoices.AddAsync(invoice, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created invoice {InvoiceId} for user {UserId}",
                invoice.Id, userId);

            return invoice.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}
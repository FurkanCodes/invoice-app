using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.Application.Features.Invoices.Commands
{
    public class CreateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IUnitOfWork unitOfWork,
        ICurrencyValidator currencyValidator,
        IUserService userService,
        ILogger<CreateInvoiceCommandHandler> logger) : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICurrencyValidator _currencyValidator = currencyValidator;
        private readonly IUserService _userService = userService;
        private readonly ILogger<CreateInvoiceCommandHandler> _logger = logger;

        public async Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
        {
            var userId = _userService.UserId;
            _logger.LogInformation("Creating invoice for user: {UserId}", userId);

            if (!_currencyValidator.IsValidCode(command.Currency))
            {
                throw new DomainException("Invalid currency code");
            }

            // Check for duplicate invoice number using repository abstraction
            if (await _invoiceRepository.InvoiceNumberExistsAsync(command.InvoiceNumber, cancellationToken))
            {
                throw new DomainException($"Invoice number '{command.InvoiceNumber}' already exists.");
            }


            // Validate user exists
            // var userExists = await _context.Users
            //     .AnyAsync(u => u.Id == userId, cancellationToken);

            // if (!userExists)
            // {
            //     throw new DomainException($"User with ID {userId} not found");
            // }

            // // Check for duplicate invoice number
            // var invoiceNumberExists = await _context.Invoices
            //     .AnyAsync(i => i.InvoiceNumber == command.InvoiceNumber, cancellationToken);

            // if (invoiceNumberExists)
            // {
            //     throw new DomainException($"Invoice number '{command.InvoiceNumber}' already exists.");
            // }

            // if (command.CustomerId.HasValue)
            // {
            //     var customerExists = await _context.Customers
            //         .AnyAsync(c => c.Id == command.CustomerId.Value, cancellationToken);
            //     if (!customerExists)
            //     {
            //         throw new DomainException($"Customer with ID {command.CustomerId} not found.");
            //     }
            // }
            // // Create the invoice
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
                command.CustomerId // Optional customer info
            );

            await _invoiceRepository.AddAsync(invoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created invoice {InvoiceId} for user {UserId}", invoice.Id, userId);

            return invoice.Id;
        }
    }
}



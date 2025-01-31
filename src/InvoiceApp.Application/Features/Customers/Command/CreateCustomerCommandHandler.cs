using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Features.Customers.Commands;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Application.Features.Customers.Command;

public class CreateCustomerCommandHandler(
    IApplicationDbContext context,
    ILogger<CreateCustomerCommandHandler> logger,
    IUserService userService)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var userId = userService.UserId;
        try
        {
            if (command.Type == CustomerType.Organization && string.IsNullOrEmpty(command.OrganizationName))
            {
                throw new DomainException("Organization name is required for organization customers");
            }

            if (command.Type == CustomerType.Person &&
                (string.IsNullOrEmpty(command.FirstName) || string.IsNullOrEmpty(command.LastName)))
            {
                throw new DomainException("First name and last name are required for individual customers");
            }
            var emailExists = await context.Customers
                .AnyAsync(c => c.UserId == command.UserId && c.Email == command.Email, cancellationToken);

            if (emailExists)
            {
                throw new DomainException($"Customer with email '{command.Email}' already exists.");
            }

            var customer = new Customer
            {
                UserId = userId,
                Type = command.Type,
                OrganizationName = command.OrganizationName,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Phone = command.Phone,
                StreetAddress = command.StreetAddress,
                City = command.City,
                State = command.State,
                PostalCode = command.PostalCode,
                Country = command.Country
            };

            await context.Customers.AddAsync(customer, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating customer: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}

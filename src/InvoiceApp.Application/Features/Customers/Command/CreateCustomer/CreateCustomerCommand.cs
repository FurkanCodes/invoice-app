using MediatR;
using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Features.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<Guid>
    {

        public Guid UserId { get; set; }

        public CustomerType Type { get; set; }
        public string? OrganizationName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PaymentTerms { get; set; } = "NET30";
        public string DefaultCurrency { get; set; } = "USD";
        public string? TaxId { get; set; }
        public string? AccountNumber { get; set; }
    }
}

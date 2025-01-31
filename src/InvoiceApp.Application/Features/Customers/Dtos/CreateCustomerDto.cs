using System.ComponentModel.DataAnnotations;
using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Features.Customers.Dtos
{
    public class CreateCustomerDto
    {
        [Required]
        public CustomerType Type { get; set; }

        [StringLength(100)]
        public string? OrganizationName { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Url]
        [StringLength(100)]
        public string? Website { get; set; }

        [Required]
        [StringLength(200)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [StringLength(50)]
        public string PaymentTerms { get; set; } = "NET30";

        [StringLength(3)]
        public string DefaultCurrency { get; set; } = "USD";

        [StringLength(50)]
        public string? TaxId { get; set; }

        [StringLength(50)]
        public string? AccountNumber { get; set; }
    }
}

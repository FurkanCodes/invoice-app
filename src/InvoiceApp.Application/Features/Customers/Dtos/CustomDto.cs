using InvoiceApp.Domain.Entities;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public string? OrganizationName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? PaymentTerms { get; set; }
    public string? DefaultCurrency { get; set; }
    public string? TaxId { get; set; }
    public string? AccountNumber { get; set; }

    public static implicit operator CustomerDto(ApiResponse<Customer> v)
    {
        throw new NotImplementedException();
    }
}

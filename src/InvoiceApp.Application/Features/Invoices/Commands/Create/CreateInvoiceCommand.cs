using MediatR;

namespace InvoiceApp.Application.Features.Invoices.Commands;

public class CreateInvoiceCommand : IRequest<Guid>
{
    public required string ClientName { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public string CompanyRegistration { get; set; } = string.Empty;
    public string LegalAddress { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public decimal TaxRate { get; set; }
    public string PaymentTerms { get; set; } = "NET30";
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }


}
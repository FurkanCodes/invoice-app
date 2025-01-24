using System;
using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Application.Features.Invoices.Commands.Delete;

public class DeleteInvoiceCommand
{
    [Required(ErrorMessage = "Invoice Id is required")]
    public string InvoiceId { get; set;}= string.Empty;

}

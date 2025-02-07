// GetInvoicesByCustomerIdHandler.cs
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Features.Invoices.Queries; // Your Invoice DTOs are here
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic; // Add this for List<T>

public class GetInvoicesByCustomerIdHandler : IRequestHandler<GetInvoicesByCustomerIdQuery, PagedResponse<InvoiceDto>>
{
    private readonly ICustomerRepository _customerRepository; // Correct interface

    // Remove the IMapper dependency
    public GetInvoicesByCustomerIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResponse<InvoiceDto>> Handle(GetInvoicesByCustomerIdQuery request, CancellationToken cancellationToken)
    {

        var pagedResponse = await _customerRepository.GetInvoicesByCustomerIdAsync(
            request.CustomerId, request.PageNumber, request.PageSize, cancellationToken);


        var invoiceDtos = new List<InvoiceDto>();
        foreach (var invoice in pagedResponse.Items)
        {
            invoiceDtos.Add(new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                ClientName = invoice.ClientName,
                CustomerId = invoice.CustomerId,
                Currency = invoice.Currency,

                TaxAmount = invoice.TaxAmount,
                TaxRate = invoice.TaxRate,
                Amount = invoice.Amount,

                PaymentTerms = invoice.PaymentTerms,


                LegalAddress = invoice.LegalAddress,
                UserId = invoice.UserId,
                DeletedAt = invoice.DeletedAt,
                IsDeleted = invoice.IsDeleted,


            });
        }


        return new PagedResponse<InvoiceDto>
        {
            Items = invoiceDtos,
            TotalCount = pagedResponse.TotalCount,
            PageNumber = pagedResponse.PageNumber,
            PageSize = pagedResponse.PageSize
        };
    }
}
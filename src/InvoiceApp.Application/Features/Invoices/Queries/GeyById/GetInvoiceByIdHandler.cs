using MediatR;

using InvoiceApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Common.Interfaces.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById
{
    public class GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
            : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(
            GetInvoiceByIdQuery query,
            CancellationToken ct)
        {
            // Fetch invoice from database
            var invoice = await invoiceRepository.GetByIdAsync(query.InvoiceId, ct);


            return new InvoiceDto
            {
                Id = invoice.Id,
                ClientName = invoice.ClientName,
                Amount = invoice.Amount,
                DueDate = invoice.DueDate
            };
        }
    }
}
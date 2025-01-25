using MediatR;

using InvoiceApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById
{
    public class GetInvoiceByIdQueryHandler(IApplicationDbContext context)
            : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        private readonly IApplicationDbContext _context = context;

        public async Task<InvoiceDto> Handle(
            GetInvoiceByIdQuery query,
            CancellationToken ct)
        {
            // Fetch invoice from database
            var invoice = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == query.InvoiceId, ct) ?? throw new DomainException("Invoice not found");

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
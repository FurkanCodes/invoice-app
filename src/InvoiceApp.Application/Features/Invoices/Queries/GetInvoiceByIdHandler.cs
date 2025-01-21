using MediatR;

using InvoiceApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Persistence;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryHandler(AppDbContext context)
        : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly AppDbContext _context = context;

    public async Task<InvoiceDto> Handle(
        GetInvoiceByIdQuery query,
        CancellationToken ct)
    {
        // Fetch invoice from database
        var invoice = await _context.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == query.InvoiceId, ct);

        // Handle "not found" case
        if (invoice == null)
            throw new DomainException("Invoice not found");

        // Map to DTO and return
        return new InvoiceDto
        {
            Id = invoice.Id,
            ClientName = invoice.ClientName,
            Amount = invoice.Amount,
            DueDate = invoice.DueDate
        };
    }
}
// GetAllInvoicesHandler.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Infrastructure.Persistence;
using InvoiceApp.Domain.Exceptions;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

// GetAllInvoicesHandler.cs
public class GetAllInvoicesHandler(AppDbContext context) 
    : IRequestHandler<GetAllInvoicesQuery, PagedResponse<InvoiceDto>>
{
    public async Task<PagedResponse<InvoiceDto>> Handle(
        GetAllInvoicesQuery query, 
        CancellationToken ct)
    {
        // Validate input
        if (query.PageNumber < 1) query.PageNumber = 1;
        if (query.PageSize < 1) query.PageSize = 10;

        // Base query
        var baseQuery = context.Invoices.AsNoTracking();

        // Get total count (without pagination)
        var totalCount = await baseQuery.CountAsync(ct);

        // Apply pagination
        var items = await baseQuery
            .OrderBy(i => i.DueDate) // Always order before pagination!
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new InvoiceDto 
            {
                Id = i.Id,
                ClientName = i.ClientName,
                Amount = i.Amount,
                DueDate = i.DueDate
            })
            .ToListAsync(ct);

        return new PagedResponse<InvoiceDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
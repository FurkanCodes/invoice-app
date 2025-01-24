using MediatR;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceApp.Application.Features.Invoices.Queries.GetDeletedInvoices;

public class GetDeletedInvoicesHandler(AppDbContext context) 
    : IRequestHandler<GetDeletedInvoicesQuery, PagedResponse<InvoiceDto>>
{
   public async Task<PagedResponse<InvoiceDto>> Handle(
        GetDeletedInvoicesQuery query, 
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.PageNumber < 1) query.PageNumber = 1;
        if (query.PageSize < 1) query.PageSize = 10;
         // Validate input
        if (query.PageNumber < 1) query.PageNumber = 1;
        if (query.PageSize < 1) query.PageSize = 10;

        // Base query
       var baseQuery = context.Invoices; 
       
               // Get total count (without pagination)
        var totalCount = await baseQuery.CountAsync(ct);

        // Apply pagination
         var items = await baseQuery
            .OrderBy(i => i.DeletedAt) // Always order before pagination!
            .IgnoreQueryFilters()
            .Where(i => i.IsDeleted)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new InvoiceDto 
            {
                Id = i.Id,
                ClientName = i.ClientName,
                Amount = i.Amount,
                DueDate = i.DueDate,
                DeletedAt = i.DeletedAt,
                IsDeleted = i.IsDeleted,
            })
            .ToListAsync(ct);
            
        return new PagedResponse<InvoiceDto>
        {
            Items = items,
            TotalCount = items.Count(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
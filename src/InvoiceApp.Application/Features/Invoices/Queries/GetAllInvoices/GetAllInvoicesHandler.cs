// GetAllInvoicesHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;

using InvoiceApp.Domain.Exceptions;
using System.Linq.Expressions;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;
namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

public class GetAllInvoicesHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllInvoicesQuery, PagedResponse<InvoiceDto>>
{
    public async Task<PagedResponse<InvoiceDto>> Handle(
        GetAllInvoicesQuery query,
        CancellationToken ct)
    {
        // Validate input
        if (query.PageNumber < 1) query.PageNumber = 1;
        if (query.PageSize < 1) query.PageSize = 10;
        if (query.StartDate > query.EndDate)
            throw new DomainException("End date cannot be before start date");

        // Base query with date filtering
        var baseQuery = context.Invoices.AsNoTracking();

        // Apply date filters
        if (query.StartDate.HasValue)
            baseQuery = baseQuery.Where(i => i.DueDate >= query.StartDate.Value.ToUniversalTime());

        if (query.EndDate.HasValue)
            baseQuery = baseQuery.Where(i => i.DueDate <= query.EndDate.Value.ToUniversalTime());

        // Get total count
        var totalCount = await baseQuery.CountAsync(ct);

        // Apply pagination and ordering
        var items = await baseQuery
            .OrderBy(i => i.DueDate)
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
            PageSize = query.PageSize,

        };
    }
}
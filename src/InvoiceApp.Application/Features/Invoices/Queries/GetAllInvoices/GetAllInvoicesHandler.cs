// GetAllInvoicesHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;

using InvoiceApp.Domain.Exceptions;
using System.Linq.Expressions;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Domain.Entities;
namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

public class GetAllInvoicesHandler(IInvoiceRepository invoiceRepository)
    : IRequestHandler<GetAllInvoicesQuery, PagedResponse<Invoice>>
{
    public async Task<PagedResponse<Invoice>> Handle(
        GetAllInvoicesQuery query,
        CancellationToken ct)
    {
        // Validate input
        if (query.PageNumber < 1) query.PageNumber = 1;
        if (query.PageSize < 1) query.PageSize = 10;
        if (query.StartDate > query.EndDate)
            throw new DomainException("End date cannot be before start date");

        return await invoiceRepository.GetAllInvoices(query.PageNumber, query.PageSize, ct);
    }
}
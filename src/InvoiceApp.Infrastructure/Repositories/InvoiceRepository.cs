using System.Net;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Infrastructure.Repositories
{
    public class InvoiceRepository(AppDbContext context) : IInvoiceRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Invoice> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);


        }

        public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            await _context.Invoices.AddAsync(invoice, cancellationToken);
        }

        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, CancellationToken cancellationToken)
        {
            return await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
        }


        public async Task<ApiResponse<AuthResponseDto>> SoftDeleteAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            if (invoice != null)
            {
                invoice.IsDeleted = true;
                invoice.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }


            return new ApiResponse<AuthResponseDto>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Invoice soft deleted successfully"
            };
        }



        public async Task<PagedResponse<Invoice>> GetDeletedInvoicesAsync(
           int pageNumber,
           int pageSize,
           CancellationToken cancellationToken)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var baseQuery = _context.Invoices
                .IgnoreQueryFilters()
                .Where(i => i.IsDeleted);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(i => i.DeletedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<Invoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PagedResponse<Invoice>> GetAllInvoices(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var baseQuery = _context.Invoices
                .Where(i => !i.IsDeleted); // Filter out deleted invoices

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                // .OrderBy(i => i.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResponse<Invoice>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

    }
}

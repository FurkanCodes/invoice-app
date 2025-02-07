using System.Net;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Infrastructure.Repositories
{
    public class CustomerRepository(AppDbContext context) : ICustomerRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken) => await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<bool> EmailExistsForUserAsync(Guid userId, string email, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .AnyAsync(c => c.UserId == userId && c.Email == email, cancellationToken);
        }

        public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _context.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<ApiResponse<AuthResponseDto>> SoftDeleteAsync(Customer customer, CancellationToken cancellationToken)
        {
            if (customer != null)
            {
                customer.IsDeleted = true;
                customer.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }


            return new ApiResponse<AuthResponseDto>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Message = "customer soft deleted successfully"
            };
        }
        public async Task<PagedResponse<Customer>> GetAllCustomers(int pageNumber, int pageSize, Guid userId, CancellationToken cancellationToken) // Add userId
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var baseQuery = _context.Customers
                .Where(i => !i.IsDeleted && i.UserId == userId);
            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var items = await baseQuery

                 .OrderBy(i => i.CreatedAt)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .AsNoTracking()
                 .ToListAsync(cancellationToken);

            return new PagedResponse<Customer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResponse<Invoice>> GetInvoicesByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            // Notice NO .Include(i => i.Customer) here.  We get customer data separately if needed.
            var baseQuery = _context.Invoices
                .Where(i => i.CustomerId == customerId && !i.IsDeleted);

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var items = await baseQuery
                 .OrderBy(i => i.IssueDate) // Or your desired ordering
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

        public async Task<ApiResponse<Customer>> GetCustomerById(Guid customerId, CancellationToken cancellationToken)
        {
            var baseQuery = _context.Invoices
                    .Where(i => i.CustomerId == customerId && !i.IsDeleted);

            var customer = await baseQuery.FirstOrDefaultAsync(i => i.Id == customerId);

            return new ApiResponse<Customer>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Success"

            };
        }
    }
}
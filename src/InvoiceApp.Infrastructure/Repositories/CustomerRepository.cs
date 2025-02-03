using System.Net;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Features.Auth.DTOs;
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


    }
}
// ICustomerRepository.cs
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Invoices.Queries; // Make sure this is present
using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Common.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> EmailExistsForUserAsync(Guid userId, string email, CancellationToken cancellationToken);
        Task AddAsync(Customer customer, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);

        Task<ApiResponse<AuthResponseDto>> SoftDeleteAsync(Customer customer, CancellationToken cancellationToken);
        Task<PagedResponse<Customer>> GetAllCustomers(int pageNumber, int pageSize, Guid userId, CancellationToken cancellationToken);

        Task<PagedResponse<Invoice>> GetInvoicesByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize, CancellationToken cancellationToken); // Correct return type (Invoice, not InvoiceDto)
    }
}
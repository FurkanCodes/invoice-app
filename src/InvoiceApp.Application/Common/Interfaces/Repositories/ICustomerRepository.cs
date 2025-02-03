using InvoiceApp.Application.Features.Auth.DTOs;
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

    }
}
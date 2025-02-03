// IInvoiceRepository.cs
using InvoiceApp.Application.Features.Auth.DTOs;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Domain.Entities;

namespace InvoiceApp.Application.Common.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber, CancellationToken cancellationToken);

        Task<PagedResponse<Invoice>> GetDeletedInvoicesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<PagedResponse<Invoice>> GetAllInvoices(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<ApiResponse<AuthResponseDto>>  SoftDeleteAsync(Invoice invoice, CancellationToken cancellationToken);

    }
}
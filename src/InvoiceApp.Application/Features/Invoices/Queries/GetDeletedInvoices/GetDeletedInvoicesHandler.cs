// GetDeletedInvoicesHandler.cs
using InvoiceApp.Application.Common.Interfaces.Repositories;
using MediatR;


namespace InvoiceApp.Application.Features.Invoices.Queries.GetDeletedInvoices
{
    public class GetDeletedInvoicesHandler(IInvoiceRepository invoiceRepository)
        : IRequestHandler<GetDeletedInvoicesQuery, PagedResponse<InvoiceDto>>
    {

        public async Task<PagedResponse<InvoiceDto>> Handle(
            GetDeletedInvoicesQuery query,
            CancellationToken ct)
        {
            query.PageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            query.PageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var pagedInvoices = await invoiceRepository.GetDeletedInvoicesAsync(
                query.PageNumber,
                query.PageSize,
                ct);

            return new PagedResponse<InvoiceDto>
            {
                Items = [.. pagedInvoices.Items.Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    ClientName = i.ClientName,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    DeletedAt = i.DeletedAt,
                    IsDeleted = i.IsDeleted
                })],
                TotalCount = pagedInvoices.TotalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
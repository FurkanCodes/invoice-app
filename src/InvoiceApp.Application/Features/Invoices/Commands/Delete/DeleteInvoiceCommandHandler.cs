// DeleteInvoiceCommandHandler.cs
using MediatR;

using InvoiceApp.Application.Common.Interfaces.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Commands
{
    public class DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        : IRequestHandler<DeleteInvoiceCommand, Unit>
    {
        public async Task<Unit> Handle(
            DeleteInvoiceCommand command,
            CancellationToken ct)
        {
            var invoice = await invoiceRepository.GetByIdAsync(command.InvoiceId, ct)
                ?? throw new ArgumentException("Invoice not found");

            await invoiceRepository.SoftDeleteAsync(invoice, ct);

            return Unit.Value;
        }
    }
}
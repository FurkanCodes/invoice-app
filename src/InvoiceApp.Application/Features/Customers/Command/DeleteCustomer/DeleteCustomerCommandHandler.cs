// DeleteInvoiceCommandHandler.cs
using MediatR;

using InvoiceApp.Application.Common.Interfaces.Repositories;

namespace InvoiceApp.Application.Features.Invoices.Commands
{
    public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        : IRequestHandler<DeleteCustomerCommand, Unit>
    {
        public async Task<Unit> Handle(
            DeleteCustomerCommand command,
            CancellationToken ct)
        {
            var invoice = await customerRepository.GetByIdAsync(command.CustomerId, ct)
                ?? throw new ArgumentException("Invoice not found");

            await customerRepository.SoftDeleteAsync(invoice, ct);

            return Unit.Value;
        }

    }
}
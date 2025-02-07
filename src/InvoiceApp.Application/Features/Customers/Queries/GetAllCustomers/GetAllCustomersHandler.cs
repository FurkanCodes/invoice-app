// GetAllInvoicesHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;

using InvoiceApp.Domain.Exceptions;
using System.Linq.Expressions;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;
namespace InvoiceApp.Application.Features.Invoices.Queries.GetAllCustomers
{
    public class GetAllCustomersHandler(ICustomerRepository customerRepository, IUserService userService)
: IRequestHandler<GetAllCustomersQuery, PagedResponse<Customer>>
    {


        public async Task<PagedResponse<Customer>> Handle(
            GetAllCustomersQuery query,
            CancellationToken ct)
        {
            // Validate input
            if (query.PageNumber < 1) query.PageNumber = 1;
            if (query.PageSize < 1) query.PageSize = 10;


            Guid userId = userService.UserId;

            return await customerRepository.GetAllCustomers(query.PageNumber, query.PageSize, userId, ct); // Pass userId
        }
    }
}
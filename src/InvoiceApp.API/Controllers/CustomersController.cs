using InvoiceApp.Application.Features.Customers.Commands;
using InvoiceApp.Application.Features.Customers.Dtos;
using InvoiceApp.Application.Features.Invoices.Commands;
using InvoiceApp.Application.Features.Invoices.Queries;
using InvoiceApp.Application.Features.Invoices.Queries.GetAllCustomers;
using InvoiceApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController(IMediator mediator, IUserService userService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var command = new CreateCustomerCommand
            {
                // Set UserId from authenticated context
                UserId = _userService.UserId,
                // Map other properties
                Type = dto.Type,
                OrganizationName = dto.OrganizationName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                PaymentTerms = dto.PaymentTerms,
                DefaultCurrency = dto.DefaultCurrency,
                TaxId = dto.TaxId,
                AccountNumber = dto.AccountNumber
            };

            var customerId = await _mediator.Send(command);
            return Ok(customerId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var command = new DeleteCustomerCommand { CustomerId = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Customer>), 200)]
        public async Task<IActionResult> GetAllCustomers(
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10
     )
        {
            var query = new GetAllCustomersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,

            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("invoices/{customerId}")]
        [ProducesResponseType(typeof(PagedResponse<InvoiceDto>), 200)] // Use a DTO!
        public async Task<IActionResult> GetInvoicesByCustomerId(
       Guid customerId,
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
        {
            // You might have a GetInvoicesByCustomerIdQuery and handler.
            var query = new GetInvoicesByCustomerIdQuery
            {
                CustomerId = customerId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

    }
}

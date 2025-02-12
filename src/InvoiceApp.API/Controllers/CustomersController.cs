using System.Net;
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
[Produces("application/json")]
public class CustomersController(IMediator mediator, IUserService userService) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IUserService _userService = userService;

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        var command = new CreateCustomerCommand
        {
            UserId = _userService.UserId,
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
        var response = new ApiResponse<Guid>
        {
            Data = customerId,
            IsSuccess = true,
            StatusCode = HttpStatusCode.Created,
            Message = "Customer created successfully"
        };
        
        return StatusCode((int)HttpStatusCode.Created, response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteCustomer(Guid id)
    {
        var command = new DeleteCustomerCommand { CustomerId = id };
        var result = await _mediator.Send(command);
        
        return Ok(new ApiResponse
        {
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Customer deleted successfully"
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<CustomerDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResponse<CustomerDto>>>> GetAllCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCustomersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        var response = new ApiResponse<PagedResponse<Customer>>
        {
            Data = result,
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Customers retrieved successfully"
        };
        
        return Ok(response);
    }

    [HttpGet("invoices/{customerId}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<InvoiceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PagedResponse<InvoiceDto>>>> GetInvoicesByCustomerId(
        Guid customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetInvoicesByCustomerIdQuery
        {
            CustomerId = customerId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        var response = new ApiResponse<PagedResponse<InvoiceDto>>
        {
            Data = result,
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Customer invoices retrieved successfully"
        };
        
        return Ok(response);
    }

    [HttpGet("{customerId}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CustomerDto>>> GetByCustomerId(Guid customerId)
    {
        var query = new GetCustomerByIdQuery
        {
            CustomerId = customerId
        };

        var result = await _mediator.Send(query);
        var response = new ApiResponse<CustomerDto>
        {
            Data = result,
            IsSuccess = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Customer retrieved successfully"
        };
        
        return Ok(response);
    }
}

}

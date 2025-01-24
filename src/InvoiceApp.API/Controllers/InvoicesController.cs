using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvoiceApp.Application.Features.Invoices.Commands;
using System.Diagnostics;
using InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;
using InvoiceApp.Application.Features.Invoices.Queries.GetDeletedInvoices;

namespace InvoiceApp.API.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        var invoiceId = await _mediator.Send(command);
        return Ok(new { InvoiceId = invoiceId });
    }
    [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
            {

                var query = new GetInvoiceByIdQuery { InvoiceId = id };
                var invoiceDto = await _mediator.Send(query);
                return Ok(invoiceDto);
        }

[HttpGet("all-invoices")]
public async Task<IActionResult> GetAllInvoices(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
{
    var query = new GetAllInvoicesQuery 
    { 
        PageNumber = pageNumber,
        PageSize = pageSize,
        StartDate = startDate,
        EndDate = endDate
    };
    
    var result = await _mediator.Send(query);
    return Ok(result);
}
  [HttpDelete("{id}")]
public async Task<IActionResult> DeleteInvoice(Guid id)
{
    var command = new DeleteInvoiceCommand { InvoiceId = id };
    await _mediator.Send(command);
    return NoContent(); // ✅ 204 status
}

    [HttpGet("all-deleted")] // 👈 Unique route
    public async Task<IActionResult> GetAllDeletedInvoices(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new GetDeletedInvoicesQuery { 
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return Ok(await _mediator.Send(query));
    }

}
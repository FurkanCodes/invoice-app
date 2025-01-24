using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvoiceApp.Application.Features.Invoices.Commands;
using System.Diagnostics;
using InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;

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
// InvoicesController.cs
[HttpGet]
public async Task<IActionResult> GetAllInvoices(
    [FromQuery] int pageNumber = 1, 
    [FromQuery] int pageSize = 10)
{
    var query = new GetAllInvoicesQuery 
    { 
        PageNumber = pageNumber,
        PageSize = pageSize
    };
    
    var result = await _mediator.Send(query);
    return Ok(result);
}
}
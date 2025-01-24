using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvoiceApp.Application.Features.Invoices.Commands;
using InvoiceApp.Application.Features.Invoices.Queries.GetAllInvoices;
using InvoiceApp.Application.Features.Invoices.Queries.GetDeletedInvoices;

using InvoiceApp.Application.Features.Invoices.Queries;

namespace InvoiceApp.API.Controllers;

/// <summary>
/// Manages invoice operations
/// </summary>
[ApiController]
[Route("api/invoices")]
[Produces("application/json")]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Creates a new invoice
    /// </summary>
    /// <param name="command">Invoice creation data</param>
    /// <returns>Created invoice ID</returns>
    /// <response code="201">Returns the newly created invoice ID</response>
    /// <response code="400">If validation fails</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        var invoiceId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvoiceById), new { id = invoiceId }, new { InvoiceId = invoiceId });
    }

    /// <summary>
    /// Gets a specific invoice by ID
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>Invoice details</returns>
    /// <response code="200">Returns the requested invoice</response>
    /// <response code="404">If invoice not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var query = new GetInvoiceByIdQuery { InvoiceId = id };
        var invoiceDto = await _mediator.Send(query);
        return Ok(invoiceDto);
    }

    /// <summary>
    /// Gets paginated list of invoices with optional date filtering
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Paginated list of invoices</returns>
    /// <response code="200">Returns the paginated list</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<InvoiceDto>), 200)]
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

    /// <summary>
    /// Soft deletes an invoice
    /// </summary>
    /// <param name="id">Invoice ID to delete</param>
    /// <response code="204">Invoice marked as deleted</response>
    /// <response code="404">If invoice not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteInvoice(Guid id)
    {
        var command = new DeleteInvoiceCommand { InvoiceId = id };
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets paginated list of soft-deleted invoices
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <returns>Paginated list of deleted invoices</returns>
    /// <response code="200">Returns the paginated list</response>
    [HttpGet("deleted")]
    [ProducesResponseType(typeof(PagedResponse<InvoiceDto>), 200)]
    public async Task<IActionResult> GetDeletedInvoices(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new GetDeletedInvoicesQuery 
        { 
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return Ok(await _mediator.Send(query));
    }
}
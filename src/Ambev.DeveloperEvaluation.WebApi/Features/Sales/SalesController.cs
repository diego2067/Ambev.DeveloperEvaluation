using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
            return BadRequest("Sale and items are required.");

        var saleId = await _mediator.Send(new CreateSaleCommand(request));
        return CreatedAtAction(nameof(GetSaleById), new { id = saleId }, saleId);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request)
    {
        var result = await _mediator.Send(new UpdateSaleCommand(id, request));
        if (!result) return NotFound("Sale not found.");
        return Ok("Sale updated successfully.");
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteSale(Guid id)
    {
        var result = await _mediator.Send(new DeleteSaleCommand(id));
        if (!result) return NotFound("Sale not found.");
        return NoContent();
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListSales([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var sales = await _mediator.Send(new ListSalesQuery(pageNumber, pageSize));
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery(id));
        if (sale == null) return NotFound("Sale not found.");
        return Ok(sale);
    }
}
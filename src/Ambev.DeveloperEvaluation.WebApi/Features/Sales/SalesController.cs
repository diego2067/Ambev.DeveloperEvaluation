using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;

        public SalesController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest("Sale and items are required.");

            var sale = new Sale
            {
                SaleNumber = request.SaleNumber,
                SaleDate = DateTime.UtcNow,
                Customer = request.Customer,
                Branch = request.Branch,
                Items = request.Items.Select(item => new SaleItem
                {
                    Product = item.Product,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            try
            {
                sale.CalculateTotal();
                await _saleRepository.AddAsync(sale);
                await _saleRepository.SaveChangesAsync();
                return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] CreateSaleRequest request)
        {
            var existingSale = await _saleRepository.GetByIdAsync(id);
            if (existingSale == null) return NotFound("Sale not found.");

            existingSale.Customer = request.Customer;
            existingSale.Branch = request.Branch;
            existingSale.Items = request.Items.Select(item => new SaleItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList();

            existingSale.CalculateTotal();
            await _saleRepository.SaveChangesAsync();
            return Ok(existingSale);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSale(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null) return NotFound("Sale not found.");

            sale.IsCancelled = true;
            await _saleRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListSales([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var sales = await _saleRepository.GetAllPagedAsync(pageNumber, pageSize);
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            if (sale == null) return NotFound("Sale not found.");
            return Ok(sale);
        }
    }

    public class CreateSaleRequest
    {
        public required string SaleNumber { get; set; }
        public required string Customer { get; set; }
        public required string Branch { get; set; }
        public required List<CreateSaleItemRequest> Items { get; set; }
    }

    public class CreateSaleItemRequest
    {
        public required string Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
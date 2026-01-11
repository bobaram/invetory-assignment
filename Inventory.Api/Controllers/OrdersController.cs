using Inventory.Api.DTOs;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IStockService _stockService;

        public OrdersController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] TransferStockDto dto)
        {
            await _stockService.TransferStockAsync(
                dto.ProductCode,
                dto.SourceWarehouseCode,
                dto.DestinationWarehouseCode,
                dto.Quantity
            );

            return Ok(new { message = "Stock transferred successfully" });
        }
    }
}

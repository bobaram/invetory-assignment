using Inventory.Api.DTOs;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public async Task<IActionResult> AddStock([FromBody] AddStockDto dto)
        {
            await _stockService.AddStockAsync(dto.ProductCode, dto.WarehouseCode, dto.Quantity);
            return Ok(new { message = "Stock added successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetStock([FromQuery] string? productCode, [FromQuery] string? warehouseCode)
        {
            if (!string.IsNullOrEmpty(productCode))
            {
                var result = await _stockService.GetStockByProductCodeAsync(productCode);
                return Ok(result);
            }

            if (!string.IsNullOrEmpty(warehouseCode))
            {
                var result = await _stockService.GetStockByWarehouseCodeAsync(warehouseCode);
                return Ok(result);
            }

            var allStocks = await _stockService.GetAllStockAsync();
            return Ok(allStocks);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.DTOs
{
    public record LoginDto(
        [Required] string Username, 
        [Required] string Password);

    public record RegisterDto(
        [Required] string Username, 
        [Required] string Password, 
        string? Role);

    public record CreateProductDto(
        [Required][MaxLength(50)] string Code, 
        [Required][MaxLength(200)] string Description);

    public record CreateWarehouseDto(
        [Required][MaxLength(50)] string Code, 
        [Required][MaxLength(100)] string Name);

    public record AddStockDto(
        [Required] string ProductCode, 
        [Required] string WarehouseCode, 
        [Range(1, int.MaxValue)] int Quantity);

    public record TransferStockDto(
        [Required] string ProductCode,
        [Required] string SourceWarehouseCode,
        [Required] string DestinationWarehouseCode,
        [Range(1, int.MaxValue)] int Quantity
    );
}
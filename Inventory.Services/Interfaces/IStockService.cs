namespace Inventory.Services.Interfaces
{
    public interface IStockService
    {
        Task AddStockAsync(string productCode, string warehouseCode, int quantity);
        Task<object> GetStockByProductCodeAsync(string productCode);
        Task<object> GetStockByWarehouseCodeAsync(string warehouseCode);
        Task<IEnumerable<object>> GetAllStockAsync();
        Task TransferStockAsync(string productCode, string sourceWarehouseCode, string destWarehouseCode, int quantity);
    }
}

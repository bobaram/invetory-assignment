namespace Inventory.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Core.Entities.Warehouse>> GetAllWarehousesAsync();
        Task<Core.Entities.Warehouse?> GetWarehouseByIdAsync(int id);
        Task<Core.Entities.Warehouse> CreateWarehouseAsync(string code, string name);
    }
}

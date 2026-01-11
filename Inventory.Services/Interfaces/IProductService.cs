namespace Inventory.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Core.Entities.Product>> GetAllProductsAsync();
        Task<Core.Entities.Product?> GetProductByIdAsync(int id);
        Task<Core.Entities.Product> CreateProductAsync(string code, string description);
    }
}

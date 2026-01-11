using Inventory.Core.Interface.Repositories;
using Inventory.Services.Interfaces;
using ProductEntity = Inventory.Core.Entities.Product;

namespace Inventory.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync()
        {
            return await _unitOfWork.Repository<ProductEntity>().GetAllAsync();
        }

        public async Task<ProductEntity?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            return product;
        }

        public async Task<ProductEntity> CreateProductAsync(string code, string description)
        {
            var repo = _unitOfWork.Repository<ProductEntity>();
            var existing = await repo.GetByCodeAsync(code);

            if (existing != null)
                throw new InvalidOperationException("Product code already exists");

            var product = new ProductEntity
            {
                Code = code,
                Description = description
            };

            await repo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product;
        }
    }
}

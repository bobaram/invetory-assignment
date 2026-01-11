using Inventory.Core.Interface.Repositories;
using Inventory.Services.Interfaces;
using ProductEntity = Inventory.Core.Entities.Product;
using WarehouseEntity = Inventory.Core.Entities.Warehouse;
using StockEntity = Inventory.Core.Entities.Stock;

namespace Inventory.Services.Stock
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddStockAsync(string productCode, string warehouseCode, int quantity)
        {
            var productRepo = _unitOfWork.Repository<ProductEntity>();
            var warehouseRepo = _unitOfWork.Repository<WarehouseEntity>();
            var stockRepo = _unitOfWork.Repository<StockEntity>();

            var product = await productRepo.GetByCodeAsync(productCode);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            var warehouse = await warehouseRepo.GetByCodeAsync(warehouseCode);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            var existingStocks = await stockRepo.FindAsync(s => s.ProductId == product.Id && s.WarehouseId == warehouse.Id);
            var existingStock = existingStocks.FirstOrDefault();

            if (existingStock != null)
            {
                existingStock.Quantity += quantity;
            }
            else
            {
                var stock = new StockEntity
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = quantity
                };
                await stockRepo.AddAsync(stock);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<object> GetStockByProductCodeAsync(string productCode)
        {
            var productRepo = _unitOfWork.Repository<ProductEntity>();
            var stockRepo = _unitOfWork.Repository<StockEntity>();

            var product = await productRepo.GetByCodeAsync(productCode);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            var stocks = await stockRepo.FindAsync(s => s.ProductId == product.Id);
            return stocks.Select(s => new
            {
                productCode,
                warehouseCode = s.Warehouse?.Code,
                warehouseName = s.Warehouse?.Name,
                quantity = s.Quantity
            });
        }

        public async Task<object> GetStockByWarehouseCodeAsync(string warehouseCode)
        {
            var warehouseRepo = _unitOfWork.Repository<WarehouseEntity>();
            var stockRepo = _unitOfWork.Repository<StockEntity>();

            var warehouse = await warehouseRepo.GetByCodeAsync(warehouseCode);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            var stocks = await stockRepo.FindAsync(s => s.WarehouseId == warehouse.Id);
            return stocks.Select(s => new
            {
                productCode = s.Product?.Code,
                productDescription = s.Product?.Description,
                warehouseCode,
                quantity = s.Quantity
            });
        }

        public async Task<IEnumerable<object>> GetAllStockAsync()
        {
            var stockRepo = _unitOfWork.Repository<StockEntity>();
            var allStocks = await stockRepo.GetAllAsync();
            return allStocks.Select(s => new
            {
                productCode = s.Product?.Code,
                warehouseCode = s.Warehouse?.Code,
                quantity = s.Quantity
            });
        }

        public async Task TransferStockAsync(string productCode, string sourceWarehouseCode, string destWarehouseCode, int quantity)
        {
            if (sourceWarehouseCode == destWarehouseCode)
                throw new ArgumentException("Source and destination cannot be the same.");

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var productRepo = _unitOfWork.Repository<ProductEntity>();
                var warehouseRepo = _unitOfWork.Repository<WarehouseEntity>();
                var stockRepo = _unitOfWork.Repository<StockEntity>();

                var product = await productRepo.GetByCodeAsync(productCode);
                if (product == null) throw new KeyNotFoundException($"Product '{productCode}' not found");

                var sourceWh = await warehouseRepo.GetByCodeAsync(sourceWarehouseCode);
                if (sourceWh == null) throw new KeyNotFoundException($"Source Warehouse '{sourceWarehouseCode}' not found");

                var destWh = await warehouseRepo.GetByCodeAsync(destWarehouseCode);
                if (destWh == null) throw new KeyNotFoundException($"Destination Warehouse '{destWarehouseCode}' not found");

                var sourceStocks = await stockRepo.FindAsync(s => s.ProductId == product.Id && s.WarehouseId == sourceWh.Id);
                var sourceStock = sourceStocks.FirstOrDefault();

                if (sourceStock == null || sourceStock.Quantity < quantity)
                    throw new InvalidOperationException($"Insufficient stock in source warehouse. Available: {sourceStock?.Quantity ?? 0}");

                var destStocks = await stockRepo.FindAsync(s => s.ProductId == product.Id && s.WarehouseId == destWh.Id);
                var destStock = destStocks.FirstOrDefault();

                if (destStock == null)
                {
                    destStock = new StockEntity
                    {
                        ProductId = product.Id,
                        WarehouseId = destWh.Id,
                        Quantity = 0
                    };
                    await stockRepo.AddAsync(destStock);
                }

                sourceStock.Quantity -= quantity;
                destStock.Quantity += quantity;

                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
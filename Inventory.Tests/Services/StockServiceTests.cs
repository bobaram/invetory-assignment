using Inventory.Core.Entities;
using Inventory.Core.Interface.Repositories;
using Inventory.Services.Stock;
using Moq;

namespace Inventory.Tests.Services
{
    public class StockServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Product>> _productRepoMock;
        private readonly Mock<IRepository<Warehouse>> _warehouseRepoMock;
        private readonly Mock<IRepository<Stock>> _stockRepoMock;
        private readonly StockService _stockService;

        public StockServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IRepository<Product>>();
            _warehouseRepoMock = new Mock<IRepository<Warehouse>>();
            _stockRepoMock = new Mock<IRepository<Stock>>();

            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(_productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Warehouse>()).Returns(_warehouseRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Stock>()).Returns(_stockRepoMock.Object);

            _stockService = new StockService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task TransferStockAsync_SufficientStock_SuccessfulTransfer()
        {
            var product = new Product { Id = 1, Code = "P001", Description = "Product 1" };
            var sourceWarehouse = new Warehouse { Id = 1, Code = "WH001", Name = "Warehouse 1" };
            var destWarehouse = new Warehouse { Id = 2, Code = "WH002", Name = "Warehouse 2" };
            var sourceStock = new Stock { ProductId = 1, WarehouseId = 1, Quantity = 100 };
            var destStock = new Stock { ProductId = 1, WarehouseId = 2, Quantity = 50 };

            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(product);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync(sourceWarehouse);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH002")).ReturnsAsync(destWarehouse);
            _stockRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Stock, bool>>>()))
                .ReturnsAsync((System.Linq.Expressions.Expression<Func<Stock, bool>> predicate) =>
                {
                    var func = predicate.Compile();
                    if (func(sourceStock)) return new List<Stock> { sourceStock };
                    if (func(destStock)) return new List<Stock> { destStock };
                    return new List<Stock>();
                });

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            await _stockService.TransferStockAsync("P001", "WH001", "WH002", 30);

            Assert.Equal(70, sourceStock.Quantity);
            Assert.Equal(80, destStock.Quantity);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task TransferStockAsync_InsufficientStock_ThrowsInvalidOperationException()
        {
            var product = new Product { Id = 1, Code = "P001", Description = "Product 1" };
            var sourceWarehouse = new Warehouse { Id = 1, Code = "WH001", Name = "Warehouse 1" };
            var destWarehouse = new Warehouse { Id = 2, Code = "WH002", Name = "Warehouse 2" };
            var sourceStock = new Stock { ProductId = 1, WarehouseId = 1, Quantity = 10 };

            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(product);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync(sourceWarehouse);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH002")).ReturnsAsync(destWarehouse);
            _stockRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Stock, bool>>>()))
                .ReturnsAsync((System.Linq.Expressions.Expression<Func<Stock, bool>> predicate) =>
                {
                    var func = predicate.Compile();
                    if (func(sourceStock)) return new List<Stock> { sourceStock };
                    return new List<Stock>();
                });

            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _stockService.TransferStockAsync("P001", "WH001", "WH002", 50));

            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task TransferStockAsync_SameSourceAndDestination_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _stockService.TransferStockAsync("P001", "WH001", "WH001", 10));
        }

        [Fact]
        public async Task TransferStockAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByCodeAsync("INVALID")).ReturnsAsync((Product?)null);
            _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _stockService.TransferStockAsync("INVALID", "WH001", "WH002", 10));
        }

        [Fact]
        public async Task AddStockAsync_NewStock_CreatesStockEntry()
        {
            var product = new Product { Id = 1, Code = "P001", Description = "Product 1" };
            var warehouse = new Warehouse { Id = 1, Code = "WH001", Name = "Warehouse 1" };

            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(product);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync(warehouse);
            _stockRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Stock, bool>>>()))
                .ReturnsAsync(new List<Stock>());
            _stockRepoMock.Setup(r => r.AddAsync(It.IsAny<Stock>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _stockService.AddStockAsync("P001", "WH001", 100);

            _stockRepoMock.Verify(r => r.AddAsync(It.Is<Stock>(s => s.Quantity == 100)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddStockAsync_ExistingStock_IncreasesQuantity()
        {
            var product = new Product { Id = 1, Code = "P001", Description = "Product 1" };
            var warehouse = new Warehouse { Id = 1, Code = "WH001", Name = "Warehouse 1" };
            var existingStock = new Stock { ProductId = 1, WarehouseId = 1, Quantity = 50 };

            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(product);
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync(warehouse);
            _stockRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Stock, bool>>>()))
                .ReturnsAsync(new List<Stock> { existingStock });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            await _stockService.AddStockAsync("P001", "WH001", 30);

            Assert.Equal(80, existingStock.Quantity);
            _stockRepoMock.Verify(r => r.AddAsync(It.IsAny<Stock>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}

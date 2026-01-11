using Inventory.Core.Entities;
using Inventory.Core.Interface.Repositories;
using Inventory.Services.Warehouse;
using Moq;

namespace Inventory.Tests.Services
{
    public class WarehouseServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Warehouse>> _warehouseRepoMock;
        private readonly WarehouseService _warehouseService;

        public WarehouseServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _warehouseRepoMock = new Mock<IRepository<Warehouse>>();
            _unitOfWorkMock.Setup(u => u.Repository<Warehouse>()).Returns(_warehouseRepoMock.Object);
            _warehouseService = new WarehouseService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllWarehousesAsync_ReturnsAllWarehouses()
        {
            var warehouses = new List<Warehouse>
            {
                new Warehouse { Id = 1, Code = "WH001", Name = "Main Warehouse" },
                new Warehouse { Id = 2, Code = "WH002", Name = "Secondary Warehouse" }
            };
            _warehouseRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(warehouses);

            var result = await _warehouseService.GetAllWarehousesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetWarehouseByIdAsync_ExistingId_ReturnsWarehouse()
        {
            var warehouse = new Warehouse { Id = 1, Code = "WH001", Name = "Main Warehouse" };
            _warehouseRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(warehouse);

            var result = await _warehouseService.GetWarehouseByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("WH001", result.Code);
            Assert.Equal("Main Warehouse", result.Name);
        }

        [Fact]
        public async Task GetWarehouseByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            _warehouseRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Warehouse?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _warehouseService.GetWarehouseByIdAsync(999));
        }

        [Fact]
        public async Task CreateWarehouseAsync_UniqueCode_CreatesWarehouse()
        {
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync((Warehouse?)null);
            _warehouseRepoMock.Setup(r => r.AddAsync(It.IsAny<Warehouse>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _warehouseService.CreateWarehouseAsync("WH001", "Main Warehouse");

            Assert.NotNull(result);
            Assert.Equal("WH001", result.Code);
            Assert.Equal("Main Warehouse", result.Name);
            _warehouseRepoMock.Verify(r => r.AddAsync(It.IsAny<Warehouse>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateWarehouseAsync_DuplicateCode_ThrowsInvalidOperationException()
        {
            var existing = new Warehouse { Id = 1, Code = "WH001", Name = "Existing" };
            _warehouseRepoMock.Setup(r => r.GetByCodeAsync("WH001")).ReturnsAsync(existing);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _warehouseService.CreateWarehouseAsync("WH001", "Duplicate"));
        }
    }
}

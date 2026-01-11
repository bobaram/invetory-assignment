using Inventory.Core.Entities;
using Inventory.Core.Interface.Repositories;
using Inventory.Services.Product;
using Moq;

namespace Inventory.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Product>> _productRepoMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IRepository<Product>>();
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(_productRepoMock.Object);
            _productService = new ProductService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Code = "P001", Description = "Product 1" },
                new Product { Id = 2, Code = "P002", Description = "Product 2" }
            };
            _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = await _productService.GetAllProductsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
        {
            var product = new Product { Id = 1, Code = "P001", Description = "Product 1" };
            _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _productService.GetProductByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("P001", result.Code);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductByIdAsync(999));
        }

        [Fact]
        public async Task CreateProductAsync_UniqueCode_CreatesProduct()
        {
            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync((Product?)null);
            _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _productService.CreateProductAsync("P001", "New Product");

            Assert.NotNull(result);
            Assert.Equal("P001", result.Code);
            Assert.Equal("New Product", result.Description);
            _productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateCode_ThrowsInvalidOperationException()
        {
            var existingProduct = new Product { Id = 1, Code = "P001", Description = "Existing" };
            _productRepoMock.Setup(r => r.GetByCodeAsync("P001")).ReturnsAsync(existingProduct);

            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _productService.CreateProductAsync("P001", "Duplicate"));
        }
    }
}

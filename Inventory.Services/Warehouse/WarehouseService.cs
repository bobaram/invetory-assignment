using Inventory.Core.Interface.Repositories;
using Inventory.Services.Interfaces;
using WarehouseEntity = Inventory.Core.Entities.Warehouse;

namespace Inventory.Services.Warehouse
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WarehouseEntity>> GetAllWarehousesAsync()
        {
            return await _unitOfWork.Repository<WarehouseEntity>().GetAllAsync();
        }

        public async Task<WarehouseEntity?> GetWarehouseByIdAsync(int id)
        {
            var warehouse = await _unitOfWork.Repository<WarehouseEntity>().GetByIdAsync(id);
            if (warehouse == null)
                throw new KeyNotFoundException("Warehouse not found");

            return warehouse;
        }

        public async Task<WarehouseEntity> CreateWarehouseAsync(string code, string name)
        {
            var repo = _unitOfWork.Repository<WarehouseEntity>();
            var existing = await repo.GetByCodeAsync(code);

            if (existing != null)
                throw new InvalidOperationException("Warehouse code already exists");

            var warehouse = new WarehouseEntity
            {
                Code = code,
                Name = name
            };

            await repo.AddAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();

            return warehouse;
        }
    }
}

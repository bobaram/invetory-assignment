using Inventory.Core.Entities;
using Inventory.Core.Interface.Repositories;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inventory.Infrastructure.Repositories
{
    public class StockRepository : Repository<Stock>
    {
        public StockRepository(AppDbContext context) : base(context) { }

        public override async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Stock>> FindAsync(Expression<Func<Stock, bool>> predicate)
        {
            return await _dbSet
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(predicate)
                .ToListAsync();
        }
    }
}

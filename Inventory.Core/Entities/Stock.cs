using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Entities
{
    public class Stock
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
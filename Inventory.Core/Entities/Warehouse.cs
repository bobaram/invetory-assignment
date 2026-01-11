using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
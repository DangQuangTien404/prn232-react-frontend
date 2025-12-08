using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int SellerId { get; set; }
        [ForeignKey("SellerId")]
        public User Seller { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EShop.DAL.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        [MaxLength(500)]
        public string ImageUrl { get; set; }
        public int SellerId { get; set; }
        public User Seller { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
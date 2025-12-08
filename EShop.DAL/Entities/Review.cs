using System;
using System.ComponentModel.DataAnnotations;

namespace EShop.DAL.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CustomerId { get; set; }
        public User Customer { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsVisible { get; set; } = true;

        [MaxLength(500)]
        public string? SellerReply { get; set; }
        public DateTime? ReplyDate { get; set; }
    }
}
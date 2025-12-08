using DAL.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ProductId { get; set; }

        // Code của bạn đang dùng tên này
        public int CustomerId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // Code của bạn đang dùng tên này
        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; }

        // --- BẮT BUỘC THÊM 2 DÒNG NÀY ĐỂ SELLER TRẢ LỜI ĐƯỢC ---
        public string? SellerReply { get; set; }
        public DateTime? ReplyDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EShop.DAL.Enums;

namespace EShop.DAL.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public User Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
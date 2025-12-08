using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public enum UserRole
    {
        Customer = 0,
        Seller = 1,
        Admin = 2
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        public ICollection<Order> Orders { get; set; } 
        public ICollection<Product> Products { get; set; } 
    }
}
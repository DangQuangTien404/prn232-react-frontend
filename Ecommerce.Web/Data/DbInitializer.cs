using BCrypt.Net;
using DAL;
using DAL.Entities;
using System;
using System.Linq;

namespace Ecommerce.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            var users = new User[]
            {
                new User{
                    Username="admin",
                    Password=BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName="Administrator",
                    Role=UserRole.Admin
                },
                new User{
                    Username="seller",
                    Password=BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName="Seller Account",
                    Role=UserRole.Seller
                },
                new User{
                    Username="customer",
                    Password=BCrypt.Net.BCrypt.HashPassword("123456"),
                    FullName="Customer Account",
                    Role=UserRole.Customer
                }
            };

            foreach (var u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

        }
    }
}
using DAL;
using DAL.Entities;

namespace Ecommerce.Web.Data
{
    public class DbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                if (!context.Users.Any(u => u.Role == UserRole.Admin))
                {
                    var admin = new User
                    {
                        Username = "admin",
                        Password = "123",
                        FullName = "System Administrator",
                        Role = UserRole.Admin
                    };

                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }
        }
    }
}
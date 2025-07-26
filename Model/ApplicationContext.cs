using LuxoraStore.Model.DB;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LuxoraStore.Model
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    Id = 1,
                    Name = "Administrator",
                    Username = "admin",
                    Email = "admin@example.com",
                    PhoneNumber = "081267874199",
                    Address = "Bandung",
                    Image = "default-product.png.jpeg",
                    Password = HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UserStatus = GeneralStatusData.GeneralStatusDataAll.Published
                }
            );
        }

        private string HashPassword(string password)
        {
            // Implementasi hashing password (gunakan BCrypt atau metode secure lainnya)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

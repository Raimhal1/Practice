using Microsoft.EntityFrameworkCore;
using Shop.Models.Store;

namespace Shop.Data
{
    public class ShopDbContext : DbContext
    {
        public DbSet<User> User { set; get; }
        public DbSet<Category> Category { set; get; }

        public ShopDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
    }
}
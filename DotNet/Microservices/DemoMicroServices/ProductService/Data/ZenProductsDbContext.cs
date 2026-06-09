using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data
{
    public class ZenProductsDbContext: DbContext
    {
        public ZenProductsDbContext(DbContextOptions<ZenProductsDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}

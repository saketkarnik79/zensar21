using Microsoft.EntityFrameworkCore;
using ASP_DemoWebAPIBasics.Models;

namespace ASP_DemoWebAPIBasics.DAL
{
    public class DemoWebAPIBasicsDbContext : DbContext
    {
        public DemoWebAPIBasicsDbContext(DbContextOptions<DemoWebAPIBasicsDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}

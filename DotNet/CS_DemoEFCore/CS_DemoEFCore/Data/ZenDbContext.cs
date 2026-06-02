using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CS_DemoEFCore.Models;

namespace CS_DemoEFCore.Data
{
    public class ZenDbContext : DbContext
    {
        public ZenDbContext(DbContextOptions<ZenDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ZenDb");
        }
    }
}

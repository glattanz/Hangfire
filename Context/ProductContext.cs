using Hangfire.Models;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
            _random = new Random();
        }

        public DbSet<Product> Products { get; set; }
        private readonly Random _random;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public Product GetRandom()
        {
            return Products.ElementAt(_random.Next(100));
        }
    }
}
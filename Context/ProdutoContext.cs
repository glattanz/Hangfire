using Hangfire.Models;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Context
{
    public class ProdutoContext : DbContext
    {
        public ProdutoContext(DbContextOptions<ProdutoContext> options) : base(options)
        {
            _random = new Random();
        }

        public DbSet<Produto> Products { get; set; }
        private readonly Random _random;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public Produto GetRandom()
        {
            return Products.ElementAt(_random.Next(100));
        }
    }
}
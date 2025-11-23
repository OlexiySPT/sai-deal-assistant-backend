using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;

namespace Sai.DealAssistant.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var product = modelBuilder.Entity<Product>();
            product.ToTable("products");
            product.HasKey(p => p.Id);

            product.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            product.Property(p => p.Price)
                   .IsRequired()
                   .HasPrecision(18, 2);
        }
    }
}
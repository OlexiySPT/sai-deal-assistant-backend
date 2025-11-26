using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using System.Reflection;

namespace Sai.DealAssistant.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Product> Products => Set<Product>();
        public virtual DbSet<SampleCustomer> SampleCustomers => Set<SampleCustomer>();
        public virtual DbSet<SampleEmployee> SampleEmployees => Set<SampleEmployee>();


        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies all IEntityTypeConfiguration<T> in the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
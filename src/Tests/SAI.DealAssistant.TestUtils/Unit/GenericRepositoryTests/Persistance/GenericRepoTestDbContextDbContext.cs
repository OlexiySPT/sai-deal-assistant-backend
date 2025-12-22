using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Entities.Samples;

namespace SAI.DealAssistant.TestUtils.Unit.GenericRepositoryTests.Persistance;

public class GenericRepoTestDbContextDbContext : DbContext
{
    public virtual DbSet<SampleEnum> SampleEnums => Set<SampleEnum>();
    public virtual DbSet<SampleCustomer> SampleCustomers => Set<SampleCustomer>();
    public virtual DbSet<SampleEmployee> SampleEmployees => Set<SampleEmployee>();

    public GenericRepoTestDbContextDbContext(DbContextOptions<GenericRepoTestDbContextDbContext> options)
        : base(options)
    {
    }
}
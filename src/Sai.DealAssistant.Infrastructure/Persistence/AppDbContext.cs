using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public virtual DbSet<User> Users => Set<User>();

    public virtual DbSet<Deal> Deals => Set<Deal>();
    public virtual DbSet<DealContactRep> DealContactReps => Set<DealContactRep>();
    public virtual DbSet<Event> Events => Set<Event>();
    public virtual DbSet<EventNote> EventNotes => Set<EventNote>();
    public virtual DbSet<EventTag> EventTags => Set<EventTag>();

    #region Read-only entities
    public virtual DbSet<EventType> EventTypes  => Set<EventType>();
    public virtual DbSet<EventState> EventStates => Set<EventState>();
    public virtual DbSet<DealState> DealStates => Set<DealState>();
    public virtual DbSet<DealType> DealTypes => Set<DealType>();
    #endregion

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure citext extension is present so columns can use citext
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
    }

    #region Optimistic Concurrency Handling

    private const int MaxRetryCount = 3;

    public override int SaveChanges()
        => SaveChangesWithConcurrencyRetryAsync(false).GetAwaiter().GetResult();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesWithConcurrencyRetryAsync(true, cancellationToken);

    private async Task<int> SaveChangesWithConcurrencyRetryAsync(
        bool async,
        CancellationToken cancellationToken = default)
    {
        for (var retry = 0; retry < MaxRetryCount; retry++)
        {
            try
            {
                return async
                    ? await base.SaveChangesAsync(cancellationToken)
                    : base.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex) when (retry < MaxRetryCount - 1)
            {
                // Resolve concurrency conflicts: refresh original values
                foreach (var entry in ex.Entries)
                {
                    var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                    entry.OriginalValues.SetValues(databaseValues);
                }

                // retry...
            }
        }

        // If max retries exceeded → throw to caller
        throw new DbUpdateConcurrencyException(
            "Max retries exceeded due to concurrency conflicts.");
    }
    #endregion
}
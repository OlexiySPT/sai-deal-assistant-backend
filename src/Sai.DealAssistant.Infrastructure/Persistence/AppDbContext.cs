using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Sai.DealAssistant.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public virtual DbSet<User> Users => Set<User>();

    public virtual DbSet<Firm> Firms => Set<Firm>();
    public virtual DbSet<Deal> Deals => Set<Deal>();
    public virtual DbSet<ContactPerson> ContactPersons => Set<ContactPerson>();
    public virtual DbSet<DealTag> DealTags => Set<DealTag>();
    public virtual DbSet<Event> Events => Set<Event>();
    public virtual DbSet<EventNote> EventNotes => Set<EventNote>();
    public virtual DbSet<AiResult> AiResults => Set<AiResult>();
    public virtual DbSet<AiRequest> AIRequests => Set<AiRequest>();

    #region Read-only entities
    public virtual DbSet<EventType> EventTypes  => Set<EventType>();
    public virtual DbSet<EventState> EventStates => Set<EventState>();
    public virtual DbSet<DealState> DealStates => Set<DealState>();
    public virtual DbSet<DealType> DealTypes => Set<DealType>();
    public virtual DbSet<AmountType> AmountTypes => Set<AmountType>();
    #endregion

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        SavingChanges += OnSavingChanges;
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        UpdateDenormFirmName();
        UpdateDenormLastActionDate();
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

    #region DenormFields Calculations
    private void UpdateDenormFirmName()
    {
        foreach (var entry in ChangeTracker.Entries<Deal>())
        {
            var firmIdProperty = entry.Property(d => d.FirmId);

            var shouldUpdate = entry.State switch
            {
                EntityState.Added => true,
                EntityState.Modified => firmIdProperty.IsModified,
                _ => false
            };

            if (!shouldUpdate)
                continue;

            var newFirmId = (int)firmIdProperty.CurrentValue!;
            var firm = Firms.Local.FirstOrDefault(f => f.Id == newFirmId)
                ?? Firms.Find(newFirmId);

            if (firm is not null)
                entry.Entity.DenormFirmName = firm.Name;
        }
    }

    private void UpdateDenormLastActionDate()
    {
        // Collect the DealIds affected by any Event add/modify/delete in this save batch.
        // For Modified/Deleted entries, read DealId from OriginalValues — the in-memory entity
        // may carry DealId = 0 when the update came from a DTO that doesn't include DealId
        // (e.g. UpdateEventCommand), and EF's SetAfterSaveBehavior.Ignore prevents it from
        // being persisted but the entity field is still overwritten in memory.
        var affectedDealIds = ChangeTracker.Entries<Event>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => e.State == EntityState.Added
                ? e.Entity.DealId
                : (int)e.OriginalValues[nameof(Event.DealId)]!)
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (affectedDealIds.Count == 0)
            return;

        foreach (var dealId in affectedDealIds)
        {
            // Build the candidate dates from the change tracker itself (Added / Modified events
            // that will survive) plus whatever is already persisted, then subtract Deleted ones.
            var deletedEventIds = ChangeTracker.Entries<Event>()
                .Where(e => e.State == EntityState.Deleted
                         && (int)e.OriginalValues[nameof(Event.DealId)]! == dealId)
                .Select(e => e.Entity.Id)
                .ToHashSet();

            // In-tracker candidates: Added/Modified events for this deal (excluding Deleted).
            // Added entries use Entity.DealId (OriginalValues not yet populated).
            // Modified entries use OriginalValues to get the true DealId.
            var trackerMax = ChangeTracker.Entries<Event>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified
                         && (e.State == EntityState.Added
                                ? e.Entity.DealId
                                : (int)e.OriginalValues[nameof(Event.DealId)]!) == dealId)
                .Select(e => e.Entity.Date.UtcDateTime)
                .DefaultIfEmpty()
                .Max();

            // Persisted candidates: existing DB rows excluding any being deleted.
            // Materialize DateTimeOffset values first — .UtcDateTime is not translatable
            // to SQL by all providers (e.g. SQLite), so the conversion is done client-side.
            var persistedMax = Events
                .Where(ev => ev.DealId == dealId && !deletedEventIds.Contains(ev.Id))
                .Select(ev => (DateTimeOffset?)ev.Date)
                .AsEnumerable()
                .Max()
                ?.UtcDateTime;

            var newMax = persistedMax.HasValue && persistedMax > trackerMax
                ? persistedMax
                : (trackerMax == default ? null : (DateTime?)trackerMax);

            // Apply to the tracked Deal entity, loading it if not already in the identity map.
            var dealEntry = ChangeTracker.Entries<Deal>()
                               .FirstOrDefault(d => d.Entity.Id == dealId)
                           ?? Entry(Deals.Local.FirstOrDefault(d => d.Id == dealId)
                                    ?? Deals.Find(dealId)!);

            dealEntry.Entity.DenormLastActionDate = newMax;

            if (dealEntry.State == EntityState.Unchanged)
                dealEntry.State = EntityState.Modified;
        }
    }
    #endregion
}
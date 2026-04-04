using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Domain.Repositories;
using Sai.DealAssistant.Infrastructure.Persistence;

namespace Sai.DealAssistant.Infrastructure.Repositories;

public class SeedRepository : ISeedRepository
{
    private readonly ILogger<SeedRepository> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly int _bulkSqlTimeoutSeconds;

    public SeedRepository(
        ILogger<SeedRepository> logger,
        AppDbContext appDbContext,
        IAppConfiguration configuration)
    {
        _logger = logger is not null ? logger : throw new ArgumentNullException(nameof(logger));
        _appDbContext = appDbContext is not null ? appDbContext : throw new ArgumentNullException(nameof(appDbContext));
        _bulkSqlTimeoutSeconds = configuration is not null ? configuration.BulkSqlTimeoutSeconds : throw new ArgumentNullException(nameof(configuration));
    }

    public async Task SeedEventTypesAsync(Func<IEnumerable<EventType>> getEventTypes)
    {
        var existing = await _appDbContext.EventTypes.ToListAsync();

        foreach (var et in getEventTypes())
        {
            if (!existing.Exists(e => string.Equals(e.Name, et.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.EventTypes.Add(et);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Event Types Enum table filled.");
    }

    public async Task SeedEventStatusesAsync(Func<IEnumerable<EventState>> getEventStates)
    {
        var existing = await _appDbContext.EventStates.ToListAsync();

        foreach (var st in getEventStates())
        {
            if (!existing.Exists(e => string.Equals(e.State, st.State, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.EventStates.Add(st);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Event Statuses Enum table filled.");
    }

    public async Task SeedDealStatesAsync(Func<IEnumerable<DealState>> getDealStates)
    {
        var existing = await _appDbContext.DealStates.ToListAsync();

        foreach (var st in getDealStates())
        {
            if (!existing.Exists(e => string.Equals(e.State, st.State, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.DealStates.Add(st);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Deal States Enum table filled.");
    }

    public async Task SeedDealTypesAsync(Func<IEnumerable<DealType>> getDealTypes)
    {
        var existing = await _appDbContext.DealTypes.ToListAsync();

        foreach (var t in getDealTypes())
        {
            if (!existing.Exists(e => string.Equals(e.Type, t.Type, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.DealTypes.Add(t);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Deal Types Enum table filled.");
    }

    public async Task SeedDealsAsync(
        Func<IEnumerable<Deal>> getDeals,
        Func<Deal, IReadOnlyList<Firm>, int?>? assignFirmId = null)
    {
        List<Deal> existing = await _appDbContext.Deals.ToListAsync();
        var amountTypes = await _appDbContext.AmountTypes.ToListAsync();

        // Load seeded firms once so the assignment delegate can use them for every new deal
        IReadOnlyList<Firm> seededFirms = assignFirmId is not null
            ? await _appDbContext.Firms.AsNoTracking().ToListAsync()
            : Array.Empty<Firm>();

        foreach (Deal d in getDeals())
        {
            if (!existing.Exists(e => string.Equals(e.Name, d.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // Set new fields with sample/default data if not already set
                if (d.AmountTypeId == null && amountTypes.Count > 0)
                    d.AmountTypeId = amountTypes.First().Id;
                if (string.IsNullOrEmpty(d.CurrencyCode))
                    d.CurrencyCode = "EUR";
                if (d.ExchangeRateToEur == null)
                    d.ExchangeRateToEur = 1.0m;
                if (d.MaxClientAmount == null)
                    d.MaxClientAmount = 100000m;
                if (d.MinClientAmount == null)
                    d.MinClientAmount = 1000m;
                if (d.ProposalAmount == null)
                    d.ProposalAmount = 50000m;

                // Assign firm at creation time if a delegate was provided
                if (assignFirmId is not null)
                    d.FirmId = assignFirmId(d, seededFirms) ?? 0;

                _appDbContext.Deals.Add(d);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Non-existing Deals created.");
    }

    public async Task SeedUsersAsync(Func<IEnumerable<User>> getUsers)
    {
        var existing = await _appDbContext.Users.ToListAsync();

        foreach (var u in getUsers())
        {
            if (!existing.Exists(e => string.Equals(e.Username, u.Username, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.Users.Add(u);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Users table filled.");
    }

    public async Task SeedEventsAsync(Func<Deal, IEnumerable<Event>> getEvents)
    {
        var deals = await _appDbContext.Deals.Include(p => p.Firm.ContactPersons).AsNoTracking().ToListAsync();

        foreach (var deal in deals)
        {
            var generatedEvents = getEvents(deal).ToArray();

            var existingEvents = await _appDbContext.Events
                .Where(e => e.DealId == deal.Id)
                .Include(e => e.Notes)
                .ToListAsync();

            foreach (var gen in generatedEvents)
            {
                var match = existingEvents.FirstOrDefault(e => e.Pos == gen.Pos);
                if (match is null)
                {
                    _appDbContext.Events.Add(gen);
                }
                else
                {
                    match.Pos = gen.Pos;
                    match.Date = gen.Date;
                    match.Topic = gen.Topic;
                    match.Agenda = gen.Agenda;
                    match.Result = gen.Result;
                    match.TypeId = gen.TypeId;
                    match.StateId = gen.StateId;
                }
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Events upsert completed.");
    }

    public async Task SeedFirmContactPersonsAsync(Func<Firm, IEnumerable<ContactPerson>> getContactPersonsForFirm)
    {
        var firms = await _appDbContext.Firms.AsNoTracking().ToListAsync();

        foreach (var firm in firms)
        {
            var generatedReps = getContactPersonsForFirm(firm).ToArray();

            var existingReps = await _appDbContext.ContactPersons
                .Where(r => r.FirmId == firm.Id)
                .ToListAsync();

            foreach (var gen in generatedReps)
            {
                // Match by Name (case-insensitive)
                var match = existingReps.FirstOrDefault(r => string.Equals(r.Name, gen.Name, StringComparison.OrdinalIgnoreCase));
                if (match is null)
                {
                    _appDbContext.ContactPersons.Add(gen);
                }
                else
                {
                    match.Position = gen.Position;
                    match.Phone = gen.Phone;
                    match.Email = gen.Email;
                    match.Description = gen.Description;
                }
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Firm contact persons upsert completed.");
    }

    // Implementation for seeding deal tags.
    public async Task SeedDealTagsAsync(Func<int, IEnumerable<DealTag>> getTagsForDeal)
    {
        if (getTagsForDeal == null) return;

        // Load existing deals
        var deals = await _appDbContext.Deals.AsNoTracking().Select(d => new { d.Id }).ToListAsync();

        if (deals.Count == 0) return;

        // Gather tags to insert
        var tagsToAdd = new List<DealTag>();

        foreach (var d in deals)
        {
            var tags = getTagsForDeal(d.Id)?.ToList();
            if (tags == null || tags.Count == 0) continue;

            // Skip inserting if any tags already exist for this deal to avoid duplicates.
            var existingForDeal = await _appDbContext.DealTags
                .AsNoTracking()
                .AnyAsync(t => t.DealId == d.Id);

            if (existingForDeal) continue;

            foreach (var tag in tags)
            {
                // Ensure a fresh entity (reset Id if present) and correct FK
                tag.Id = 0;
                tag.DealId = d.Id;
                tagsToAdd.Add(tag);
            }
        }

        if (tagsToAdd.Count == 0) return;

        await _appDbContext.DealTags.AddRangeAsync(tagsToAdd);
        await _appDbContext.SaveChangesAsync();
    }

    // Added: seed EventNotes for each Event (upsert by Order)
    public async Task SeedEventNotesAsync(Func<Event, IEnumerable<EventNote>> getNotesForEvent)
    {
        if (getNotesForEvent == null) return;

        // Load events so generator has context; use AsNoTracking to avoid EF tracking collisions.
        var events = await _appDbContext.Events.AsNoTracking().ToListAsync();

        foreach (var ev in events)
        {
            var generatedNotes = getNotesForEvent(ev)?.ToArray();
            if (generatedNotes == null || generatedNotes.Length == 0) continue;

            var existingNotes = await _appDbContext.EventNotes
                .Where(n => n.EventId == ev.Id)
                .ToListAsync();

            foreach (var gen in generatedNotes)
            {
                // Ensure FK is correct and new entities don't carry an Id that causes conflicts.
                gen.EventId = ev.Id;
                gen.Id = 0;

                // Match by Order (positional note)
                var match = existingNotes.FirstOrDefault(n => n.Order == gen.Order);
                if (match is null)
                {
                    _appDbContext.EventNotes.Add(gen);
                }
                else
                {
                    match.Order = gen.Order;
                    match.Text = gen.Text;
                }
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Event notes upsert completed.");
    }

    public async Task SeedAmountTypesAsync(Func<IEnumerable<AmountType>> getAmountTypes)
    {
        var existing = await _appDbContext.Set<AmountType>().ToListAsync();

        foreach (var at in getAmountTypes())
        {
            if (!existing.Exists(e => string.Equals(e.Type, at.Type, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.Set<AmountType>().Add(at);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Amount Types Enum table filled.");
    }

    public async Task<IReadOnlyList<Firm>> SeedFirmsAsync(Func<IEnumerable<Firm>> getFirms)
    {
        var existing = await _appDbContext.Firms.ToListAsync();

        foreach (var firm in getFirms())
        {
            if (!existing.Exists(f => string.Equals(f.Name, firm.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _appDbContext.Firms.Add(firm);
            }
        }

        await _appDbContext.SaveChangesAsync();

        _logger.LogInformation("Firms upsert completed.");

        return await _appDbContext.Firms.AsNoTracking().ToListAsync();
    }
    #region multoplication methods for test data
    private static readonly Lazy<string> _multiplyFirmsSql          = new(() => SeedSqlResource.Load("MultiplyFirms.sql"));
    private static readonly Lazy<string> _multiplyDealsSql          = new(() => SeedSqlResource.Load("MultiplyDeals.sql"));
    private static readonly Lazy<string> _multiplyContactPersonsSql = new(() => SeedSqlResource.Load("MultiplyContactPersons.sql"));
    private static readonly Lazy<string> _multiplyEventsSql         = new(() => SeedSqlResource.Load("MultiplyEvents.sql"));
    private static readonly Lazy<string> _multiplyEventNotesSql     = new(() => SeedSqlResource.Load("MultiplyEventNotes.sql"));
    private static readonly Lazy<string> _multiplyDealTagsSql       = new(() => SeedSqlResource.Load("MultiplyDealTags.sql"));
    private static readonly Lazy<string> _updateDenormFirmNameSql       = new(() => SeedSqlResource.Load("UpdateDenormFirmName.sql"));
    private static readonly Lazy<string> _updateDenormLastActionDateSql = new(() => SeedSqlResource.Load("UpdateDenormLastActionDate.sql"));

    private async Task UpdateDenormFirmNameAsync()
        => await ExecuteBulkSqlAsync(_updateDenormFirmNameSql.Value);

    private async Task UpdateDenormLastActionDateAsync()
        => await ExecuteBulkSqlAsync(_updateDenormLastActionDateSql.Value);

    private async Task ExecuteBulkSqlAsync(string sql)
    {
        var connection = (NpgsqlConnection)_appDbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, connection)
        {
            CommandTimeout = _bulkSqlTimeoutSeconds
        };
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task MultiplyFirmsAsync(int targetRowCount)
    {
        while (true)
        {
            var currentCount = await _appDbContext.Firms.CountAsync();
            if (currentCount >= targetRowCount)
            {
                break;
            }
            await ExecuteBulkSqlAsync(_multiplyFirmsSql.Value);
            await ExecuteBulkSqlAsync("analyze public.\"Firms\"");
        }
         _logger.LogInformation("Firms multiplied to reach target row count.");
    }

    /// <inheritdoc/>
    public async Task MultiplyDealsAsync(int targetRowCount)
    {
        while (true)
        {
            var currentCount = await _appDbContext.Deals.CountAsync();
            if (currentCount >= targetRowCount)
            {
                break;
            }
            await ExecuteBulkSqlAsync(_multiplyDealsSql.Value);
            await ExecuteBulkSqlAsync("analyze public.\"Deals\"");
        }
        await UpdateDenormFirmNameAsync();
    }

    /// <inheritdoc/>
    public async Task MultiplyDealDependentsAsync(int targetRowCount)
    {
        while (true)
        {
            var currentCount = await _appDbContext.ContactPersons.CountAsync();
            if (currentCount >= targetRowCount)
                break;
            await ExecuteBulkSqlAsync(_multiplyContactPersonsSql.Value);
        }
        await ExecuteBulkSqlAsync("analyze public.\"ContactPersons\"");
        _logger.LogInformation("ContactPersons multiplied to reach target row count.");

        while (true)
        {
            var currentCount = await _appDbContext.Events.CountAsync();
            if (currentCount >= targetRowCount)
                break;
            await ExecuteBulkSqlAsync(_multiplyEventsSql.Value);
        }
        await ExecuteBulkSqlAsync("analyze public.\"Events\"");
        _logger.LogInformation("Events multiplied to reach target row count.");

        while (true)
        {
            var currentCount = await _appDbContext.EventNotes.CountAsync();
            if (currentCount >= targetRowCount)
                break;
            await ExecuteBulkSqlAsync(_multiplyEventNotesSql.Value);
        }
        await ExecuteBulkSqlAsync("analyze public.\"EventNotes\"");
        _logger.LogInformation("EventNotes multiplied to reach target row count.");

        while (true)
        {
            var currentCount = await _appDbContext.DealTags.CountAsync();
            if (currentCount >= targetRowCount)
                break;
            await ExecuteBulkSqlAsync(_multiplyDealTagsSql.Value);
        }
        await ExecuteBulkSqlAsync("analyze public.\"DealTags\"");
        _logger.LogInformation("DealTags multiplied to reach target row count.");

        await UpdateDenormLastActionDateAsync();
        _logger.LogInformation("DenormLastActionDate updated.");

        await UpdateDenormFirmNameAsync();
         _logger.LogInformation("DenormFirmName updated.");

    }

    public async Task<bool> AnyTestDataExistsAsync()
    {
        return await _appDbContext.Firms.AnyAsync();
    }

    #endregion
}

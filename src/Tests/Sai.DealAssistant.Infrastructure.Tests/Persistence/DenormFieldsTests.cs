using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;

namespace Sai.DealAssistant.Infrastructure.Tests.Persistence;

/// <summary>
/// Verifies that AppDbContext.OnSavingChanges keeps DenormFirmName and
/// DenormLastActionDate consistent whenever Deals or Events are mutated.
/// Uses SQLite in-memory (same connection as UnitTestBase) so the real
/// EF entity configurations and the SavingChanges hook are exercised.
/// </summary>
public class DenormFieldsTests : UnitTestBase
{
    private readonly CrudRepository<Sai.DealAssistant.Infrastructure.Persistence.AppDbContext, Deal> _dealRepo;
    private readonly CrudRepository<Sai.DealAssistant.Infrastructure.Persistence.AppDbContext, Event> _eventRepo;

    public DenormFieldsTests() : base(seedTestData: true)
    {
        _dealRepo  = new CrudRepository<Sai.DealAssistant.Infrastructure.Persistence.AppDbContext, Deal>(DbContext);
        _eventRepo = new CrudRepository<Sai.DealAssistant.Infrastructure.Persistence.AppDbContext, Event>(DbContext);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private Firm CreateFirm(string name = "Test Firm")
    {
        var firm = new Firm { Name = name, Country = "Test" };
        DbContext.Firms.Add(firm);
        DbContext.SaveChanges();
        return firm;
    }

    private Deal CreateDeal(int firmId) =>
        _dealRepo.CreateAsync(new Deal
        {
            Name      = "Test Deal",
            FirmId    = firmId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TypeId    = DbContext.DealTypes.First().Id,
            StateId   = DbContext.DealStates.First().Id,
        }).GetAwaiter().GetResult();

    private Event CreateEvent(int dealId, DateTimeOffset date) =>
        _eventRepo.CreateAsync(new Event
        {
            DealId  = dealId,
            Date    = date,
            Topic   = "Test Event",
            TypeId  = DbContext.EventTypes.First().Id,
            StateId = DbContext.EventStates.First().Id,
        }).GetAwaiter().GetResult();

    // -------------------------------------------------------------------------
    // DenormFirmName
    // -------------------------------------------------------------------------

    [Fact]
    public void CreateDeal_SetsDenormFirmName()
    {
        var firm = CreateFirm("Alpha Corp");

        var deal = CreateDeal(firm.Id);

        Assert.Equal("Alpha Corp", deal.DenormFirmName);
    }

    [Fact]
    public async Task UpdateDeal_FirmIdUnchanged_DoesNotChangeDenormFirmName()
    {
        var firm = CreateFirm("Beta Corp");
        var deal = CreateDeal(firm.Id);

        deal.Name = "Renamed Deal";
        await _dealRepo.UpdateAsync(deal);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal("Beta Corp", reloaded.DenormFirmName);
    }

    [Fact]
    public async Task UpdateDeal_FirmIdChanged_UpdatesDenormFirmName()
    {
        var firmA = CreateFirm("Firm A");
        var firmB = CreateFirm("Firm B");
        var deal  = CreateDeal(firmA.Id);

        deal.FirmId = firmB.Id;
        await _dealRepo.UpdateAsync(deal);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal("Firm B", reloaded.DenormFirmName);
    }

    // -------------------------------------------------------------------------
    // DenormLastActionDate
    // -------------------------------------------------------------------------

    [Fact]
    public async Task CreateEvent_SetsDenormLastActionDate()
    {
        var firm      = CreateFirm();
        var deal      = CreateDeal(firm.Id);
        var eventDate = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);

        CreateEvent(deal.Id, eventDate);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal(eventDate.UtcDateTime, reloaded.DenormLastActionDate);
    }

    [Fact]
    public async Task CreateMultipleEvents_SetsDenormLastActionDateToMax()
    {
        var firm   = CreateFirm();
        var deal   = CreateDeal(firm.Id);
        var older  = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newer  = new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero);

        CreateEvent(deal.Id, older);
        CreateEvent(deal.Id, newer);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal(newer.UtcDateTime, reloaded.DenormLastActionDate);
    }

    [Fact]
    public async Task UpdateEvent_DateChanged_UpdatesDenormLastActionDate()
    {
        var firm       = CreateFirm();
        var deal       = CreateDeal(firm.Id);
        var original   = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero);
        var updated    = new DateTimeOffset(2024, 11, 1, 0, 0, 0, TimeSpan.Zero);
        var evt        = CreateEvent(deal.Id, original);

        evt.Date = updated;
        await _eventRepo.UpdateAsync(evt);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal(updated.UtcDateTime, reloaded.DenormLastActionDate);
    }

    [Fact]
    public async Task DeleteEvent_WhenOnlyEvent_ClearsDenormLastActionDate()
    {
        var firm = CreateFirm();
        var deal = CreateDeal(firm.Id);
        var evt  = CreateEvent(deal.Id, new DateTimeOffset(2024, 5, 1, 0, 0, 0, TimeSpan.Zero));

        await _eventRepo.DeleteAsync(evt.Id);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Null(reloaded.DenormLastActionDate);
    }

    [Fact]
    public async Task DeleteEvent_WhenMultipleEvents_SetsDenormLastActionDateToRemainingMax()
    {
        var firm   = CreateFirm();
        var deal   = CreateDeal(firm.Id);
        var older  = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);
        var newer  = new DateTimeOffset(2024, 8, 1, 0, 0, 0, TimeSpan.Zero);

        CreateEvent(deal.Id, older);
        var newestEvt = CreateEvent(deal.Id, newer);

        // Delete the most-recent event — the denorm should fall back to the older one.
        await _eventRepo.DeleteAsync(newestEvt.Id);

        var reloaded = await DbContext.Deals.AsNoTracking().FirstAsync(d => d.Id == deal.Id);
        Assert.Equal(older.UtcDateTime, reloaded.DenormLastActionDate);
    }
}

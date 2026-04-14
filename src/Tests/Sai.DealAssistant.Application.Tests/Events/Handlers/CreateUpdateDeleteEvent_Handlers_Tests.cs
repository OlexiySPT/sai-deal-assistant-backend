using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Events.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Events.Handlers;

public class CreateUpdateDeleteEvent_Handlers_Tests : UnitTestBase
{
    private readonly CrudRepository<Infrastructure.Persistence.AppDbContext, Event> _repository;
    private readonly CreateEventCommand.Handler _createHandler;
    private readonly UpdateEventCommand.Handler _updateHandler;
    private readonly DeleteEventCommand.Handler _deleteHandler;

    public CreateUpdateDeleteEvent_Handlers_Tests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<Infrastructure.Persistence.AppDbContext, Event>(DbContext);
        _createHandler = new CreateEventCommand.Handler(_repository, Mapper);
        _updateHandler = new UpdateEventCommand.Handler(_repository, Mapper);
        _deleteHandler = new DeleteEventCommand.Handler(_repository, Mapper);
    }

    [Fact]
    public async Task Create_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        // Deal and ContactPerson must share the same Firm for the event link to be valid
        var firmId = CreateTestFirm();
        var dealId = CreateTestDealWithFirm(firmId);
        var contactPersonId = CreateTestContactPerson(firmId);

        var cmd = new CreateEventCommand
        {
            DealId = dealId,
            Date = DateTimeOffset.UtcNow,
            Pos = 1,
            Topic = "Initial contact and discovery",
            Agenda = "Test event agenda",
            Result = "Test result",
            TypeId = 1,
            StateId = 1,
            ContactPersonId = contactPersonId
        };

        // Act
        var result = await _createHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Agenda.Should().Be("Test event agenda");
        result.Result.Should().Be("Test result");
        result.TypeId.Should().Be(1);
        result.StateId.Should().Be(1);
        result.ContactPersonId.Should().Be(contactPersonId);

        // Verify in database
        var createdEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == result.Id);
        createdEvent.Should().NotBeNull();
        createdEvent!.Agenda.Should().Be("Test event agenda");
        createdEvent.DealId.Should().Be(dealId);
        createdEvent.ContactPersonId.Should().Be(contactPersonId);
    }

    [Fact]
    public async Task Create_Handler_CreatesEventWithoutContactPerson_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();

        var cmd = new CreateEventCommand
        {
            DealId = dealId,
            Date = DateTimeOffset.UtcNow,
            Pos = 1,
            Topic = "Requirements gathering",
            Agenda = "Event without contact person",
            TypeId = 1,
            StateId = 1,
            ContactPersonId = null
        };

        // Act
        var result = await _createHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.ContactPersonId.Should().BeNull();

        // Verify in database
        var createdEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == result.Id);
        createdEvent.Should().NotBeNull();
        createdEvent!.ContactPersonId.Should().BeNull();
    }

    [Fact]
    public async Task Create_Handler_CreatesMultipleEventsForDeal_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();

        var cmd1 = new CreateEventCommand
        {
            DealId = dealId,
            Topic = "Event 1",
            Date = DateTimeOffset.UtcNow.AddDays(-5),
            Pos = 1,
            Agenda = "First event",
            TypeId = 1,
            StateId = 1
        };

        var cmd2 = new CreateEventCommand
        {
            DealId = dealId,
            Topic = "Event 2",
            Date = DateTimeOffset.UtcNow.AddDays(-3),
            Pos = 2,
            Agenda = "Second event",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result1 = await _createHandler.Handle(cmd1, CancellationToken.None);
        var result2 = await _createHandler.Handle(cmd2, CancellationToken.None);

        // Assert
        result1.Pos.Should().Be(1);
        result2.Pos.Should().Be(2);

        // Verify in database
        var events = await DbContext.Events
            .Where(e => e.DealId == dealId)
            .OrderBy(e => e.Pos)
            .ToListAsync();

        events.Should().HaveCount(2);
        events[0].Agenda.Should().Be("First event");
        events[1].Agenda.Should().Be("Second event");
    }

    [Fact]
    public async Task Update_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        // Deal and ContactPerson must share the same Firm for the event link to be valid
        var firmId = CreateTestFirm();
        var dealId = CreateTestDealWithFirm(firmId);
        var contactPersonId = CreateTestContactPerson(firmId);
        var eventId = CreateTestEvent(dealId, "Original agenda", "Original result");

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateEventCommand
        {
            Id = eventId,
            Date = DateTimeOffset.UtcNow,
            Topic = "Updated topic",
            Pos = 1,
            Agenda = "Updated agenda",
            Result = "Updated result",
            TypeId = 1,
            StateId = 1,
            ContactPersonId = contactPersonId
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventId);
        result.Agenda.Should().Be("Updated agenda");
        result.Result.Should().Be("Updated result");
        result.ContactPersonId.Should().Be(contactPersonId);

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        updatedEvent.Should().NotBeNull();
        updatedEvent!.Agenda.Should().Be("Updated agenda");
        updatedEvent.Result.Should().Be("Updated result");
    }

    [Fact]
    public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
    {
        // Arrange
        int nonExistentEventId = 999999;

        var cmd = new UpdateEventCommand
        {
            Id = nonExistentEventId,
            Date = DateTimeOffset.UtcNow,
            TypeId = 1,
            StateId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _updateHandler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Update_Handler_UpdatesPartialProperties_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId, "Original agenda", "Original result");

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateEventCommand
        {
            Id = eventId,
            Date = DateTimeOffset.UtcNow,
            Topic = "Updated topic only",
            Pos = 1,
            Agenda = "Updated agenda only",
            Result = "Original result", // Keep original
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Agenda.Should().Be("Updated agenda only");
        result.Result.Should().Be("Original result");

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        updatedEvent!.Agenda.Should().Be("Updated agenda only");
        updatedEvent.Result.Should().Be("Original result");
    }

    [Fact]
    public async Task Update_Handler_ClearsContactPerson_OnSuccess()
    {
        // Arrange
        // Deal and ContactPerson must share the same Firm for the initial event link to be valid
        var firmId = CreateTestFirm();
        var dealId = CreateTestDealWithFirm(firmId);
        var contactPersonId = CreateTestContactPerson(firmId);
        var eventId = CreateTestEventWithContactPerson(dealId, contactPersonId);

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateEventCommand
        {
            Id = eventId,
            Date = DateTimeOffset.UtcNow,
            Topic = "Event with cleared contact person",
            Pos = 1,
            Agenda = "Event agenda",
            TypeId = 1,
            StateId = 1,
            ContactPersonId = null // Clear contact person
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ContactPersonId.Should().BeNull();

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        updatedEvent!.ContactPersonId.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId, "Event to delete", "Delete result");

        // Act
        var result = await _deleteHandler.Handle(new DeleteEventCommand(eventId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventId);
        result.Agenda.Should().Be("Event to delete");
        result.Result.Should().Be("Delete result");

        // Verify deleted from database
        var deletedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        deletedEvent.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
    {
        // Arrange
        int nonExistentEventId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _deleteHandler.Handle(new DeleteEventCommand(nonExistentEventId), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_Handler_DeletesEventWithNotes_CascadesDelete()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId, "Event with notes", "Result");
        var noteId1 = CreateTestEventNote(eventId, "Note 1", 1);
        var noteId2 = CreateTestEventNote(eventId, "Note 2", 2);

        // Act
        var result = await _deleteHandler.Handle(new DeleteEventCommand(eventId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventId);

        // Verify event deleted from database
        var deletedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        deletedEvent.Should().BeNull();

        // Verify notes cascade deleted
        var deletedNote1 = await DbContext.EventNotes.FirstOrDefaultAsync(n => n.Id == noteId1);
        var deletedNote2 = await DbContext.EventNotes.FirstOrDefaultAsync(n => n.Id == noteId2);
        deletedNote1.Should().BeNull("EventNotes should be cascade deleted");
        deletedNote2.Should().BeNull("EventNotes should be cascade deleted");
    }

    [Fact]
    public async Task Delete_Handler_DeletesOneEvent_LeavesOthers()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId1 = CreateTestEvent(dealId, "Event 1", "Result 1");
        var eventId2 = CreateTestEvent(dealId, "Event 2", "Result 2");
        var eventId3 = CreateTestEvent(dealId, "Event 3", "Result 3");

        // Act - Delete the second event
        await _deleteHandler.Handle(new DeleteEventCommand(eventId2), CancellationToken.None);

        // Assert
        var remainingEvents = await DbContext.Events
            .Where(e => e.DealId == dealId)
            .ToListAsync();

        remainingEvents.Should().HaveCount(2);
        remainingEvents.Any(e => e.Id == eventId1).Should().BeTrue();
        remainingEvents.Any(e => e.Id == eventId3).Should().BeTrue();

        // Verify the deleted event is gone
        var deletedEvent = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId2);
        deletedEvent.Should().BeNull();
    }

    #region Helpers

    /// <summary>Creates a firm and returns its Id.</summary>
    private int CreateTestFirm()
    {
        var now = DateTime.UtcNow;

        var firm = new Firm
        {
            Name = "Test Firm " + Guid.NewGuid(),
            Country = "USA",
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Firms.Add(firm);
        DbContext.SaveChanges();

        return firm.Id;
    }

    /// <summary>Creates a deal assigned to the first available seeded firm.</summary>
    private int CreateTestDeal()
    {
        var now = DateTime.UtcNow;
        var firmId = DbContext.Firms.Select(f => f.Id).First();

        var deal = new Deal
        {
            Name = "Test Deal " + Guid.NewGuid(),
            Description = "Test deal for event tests",
            TypeId = 1,
            StateId = 1,
            FirmId = firmId,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }

    /// <summary>
    /// Creates a deal assigned to the given firm.
    /// Required when an event on this deal must reference a ContactPerson,
    /// because ContactPerson.FirmId must match Deal.FirmId.
    /// </summary>
    private int CreateTestDealWithFirm(int firmId)
    {
        var now = DateTime.UtcNow;

        var deal = new Deal
        {
            Name = "Test Deal " + Guid.NewGuid(),
            Description = "Test deal for event tests",
            TypeId = 1,
            StateId = 1,
            FirmId = firmId,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }

    /// <summary>
    /// Creates a contact person linked to the given firm.
    /// Use together with CreateTestDealWithFirm(firmId) so the same firmId is shared.
    /// </summary>
    private int CreateTestContactPerson(int firmId)
    {
        var now = DateTime.UtcNow;

        var contactPerson = new ContactPerson
        {
            FirmId = firmId,
            Name = "Test Contact",
            Email = "test@example.com",
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.ContactPersons.Add(contactPerson);
        DbContext.SaveChanges();

        return contactPerson.Id;
    }

    private int CreateTestEvent(int dealId, string agenda, string result)
    {
        var now = DateTime.UtcNow;

        var evt = new Event
        {
            DealId = dealId,
            Date = DateTimeOffset.UtcNow,
            Pos = 1,
            Topic = Guid.NewGuid().ToString("N")[..12],
            Agenda = agenda,
            Result = result,
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Events.Add(evt);
        DbContext.SaveChanges();

        return evt.Id;
    }

    private int CreateTestEventWithContactPerson(int dealId, int contactPersonId)
    {
        var now = DateTime.UtcNow;

        var evt = new Event
        {
            DealId = dealId,
            ContactPersonId = contactPersonId,
            Date = DateTimeOffset.UtcNow,
            Pos = 1,
            Topic = Guid.NewGuid().ToString("N")[..12],
            Agenda = "Event with contact person",
            Result = "Result",
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Events.Add(evt);
        DbContext.SaveChanges();

        return evt.Id;
    }

    private int CreateTestEventNote(int eventId, string text, int order)
    {
        var eventNote = new EventNote
        {
            EventId = eventId,
            Text = text,
            Order = order
        };

        DbContext.EventNotes.Add(eventNote);
        DbContext.SaveChanges();

        return eventNote.Id;
    }
    #endregion
}
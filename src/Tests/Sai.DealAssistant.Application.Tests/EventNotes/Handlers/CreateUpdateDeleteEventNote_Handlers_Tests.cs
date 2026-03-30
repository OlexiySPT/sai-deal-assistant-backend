using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;

namespace Tests.Sai.DealAssistant.Application.Tests.EventNotes.Handlers;

public class CreateUpdateDeleteEventNote_Handlers_Tests : UnitTestBase
{
    private readonly CrudRepository<AppDbContext, EventNote> _repository;
    private readonly CreateEventNoteCommand.Handler _createHandler;
    private readonly UpdateEventNoteCommand.Handler _updateHandler;
    private readonly DeleteEventNoteCommand.Handler _deleteHandler;

    public CreateUpdateDeleteEventNote_Handlers_Tests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<AppDbContext, EventNote>(DbContext);
        _createHandler = new CreateEventNoteCommand.Handler(_repository, Mapper);
        _updateHandler = new UpdateEventNoteCommand.Handler(_repository, Mapper);
        _deleteHandler = new DeleteEventNoteCommand.Handler(_repository, Mapper);
    }

    [Fact]
    public async Task Create_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);

        var cmd = new CreateEventNoteCommand
        {
            EventId = eventId,
            Text = "New note",
            Order = 1
        };

        // Act
        var result = await _createHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Text.Should().Be("New note");
        result.Order.Should().Be(1);
        result.EventId.Should().Be(eventId);

        // Verify in database
        var createdEventNote = await DbContext.EventNotes.FirstOrDefaultAsync(en => en.Id == result.Id);
        createdEventNote.Should().NotBeNull();
        createdEventNote!.Text.Should().Be("New note");
        createdEventNote.Order.Should().Be(1);
        createdEventNote.EventId.Should().Be(eventId);
    }

    [Fact]
    public async Task Create_Handler_CreatesMultipleNotes_WithDifferentOrders()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);

        var cmd1 = new CreateEventNoteCommand
        {
            EventId = eventId,
            Text = "First note",
            Order = 1
        };

        var cmd2 = new CreateEventNoteCommand
        {
            EventId = eventId,
            Text = "Second note",
            Order = 2
        };

        var cmd3 = new CreateEventNoteCommand
        {
            EventId = eventId,
            Text = "Third note",
            Order = 3
        };

        // Act
        var result1 = await _createHandler.Handle(cmd1, CancellationToken.None);
        var result2 = await _createHandler.Handle(cmd2, CancellationToken.None);
        var result3 = await _createHandler.Handle(cmd3, CancellationToken.None);

        // Assert
        result1.Order.Should().Be(1);
        result2.Order.Should().Be(2);
        result3.Order.Should().Be(3);

        // Verify in database
        var eventNotes = await DbContext.EventNotes
            .Where(en => en.EventId == eventId)
            .OrderBy(en => en.Order)
            .ToListAsync();

        eventNotes.Should().HaveCount(3);
        eventNotes[0].Text.Should().Be("First note");
        eventNotes[1].Text.Should().Be("Second note");
        eventNotes[2].Text.Should().Be("Third note");
    }

    [Fact]
    public async Task Update_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        var eventNoteId = CreateTestEventNote(eventId, "Original note", 1);

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateEventNoteCommand
        {
            Id = eventNoteId,
            EventId = eventId,
            Text = "Updated note",
            Order = 2
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventNoteId);
        result.Text.Should().Be("Updated note");
        result.Order.Should().Be(2);

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedEventNote = await DbContext.EventNotes.FirstOrDefaultAsync(en => en.Id == eventNoteId);
        updatedEventNote.Should().NotBeNull();
        updatedEventNote!.Text.Should().Be("Updated note");
        updatedEventNote.Order.Should().Be(2);
    }

    [Fact]
    public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        int nonExistentEventNoteId = 999999;

        var cmd = new UpdateEventNoteCommand
        {
            Id = nonExistentEventNoteId,
            EventId = eventId,
            Text = "Does Not Exist"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _updateHandler.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Update_Handler_UpdatesTextOnly_KeepsOrder()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        var eventNoteId = CreateTestEventNote(eventId, "Original note", 5);

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateEventNoteCommand
        {
            Id = eventNoteId,
            EventId = eventId,
            Text = "Updated text only",
            Order = 5 // Keep original order
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Text.Should().Be("Updated text only");
        result.Order.Should().Be(5);

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedEventNote = await DbContext.EventNotes.FirstOrDefaultAsync(en => en.Id == eventNoteId);
        updatedEventNote!.Text.Should().Be("Updated text only");
        updatedEventNote.Order.Should().Be(5);
    }

    [Fact]
    public async Task Update_Handler_ReordersNotes_Successfully()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        var eventNoteId1 = CreateTestEventNote(eventId, "First note", 1);
        var eventNoteId2 = CreateTestEventNote(eventId, "Second note", 2);

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        // Swap the order
        var cmd1 = new UpdateEventNoteCommand
        {
            Id = eventNoteId1,
            EventId = eventId,
            Text = "First note",
            Order = 2
        };

        var cmd2 = new UpdateEventNoteCommand
        {
            Id = eventNoteId2,
            EventId = eventId,
            Text = "Second note",
            Order = 1
        };

        // Act
        await _updateHandler.Handle(cmd1, CancellationToken.None);
        DbContext.ChangeTracker.Clear();
        await _updateHandler.Handle(cmd2, CancellationToken.None);

        // Assert
        DbContext.ChangeTracker.Clear();
        var notes = await DbContext.EventNotes
            .Where(en => en.EventId == eventId)
            .OrderBy(en => en.Order)
            .ToListAsync();

        notes.Should().HaveCount(2);
        notes[0].Text.Should().Be("Second note");
        notes[0].Order.Should().Be(1);
        notes[1].Text.Should().Be("First note");
        notes[1].Order.Should().Be(2);
    }

    [Fact]
    public async Task Delete_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        var eventNoteId = CreateTestEventNote(eventId, "To delete", 1);

        // Act
        var result = await _deleteHandler.Handle(new DeleteEventNoteCommand(eventNoteId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(eventNoteId);
        result.Text.Should().Be("To delete");
        result.Order.Should().Be(1);

        // Verify deleted from database
        var deletedEventNote = await DbContext.EventNotes.FirstOrDefaultAsync(en => en.Id == eventNoteId);
        deletedEventNote.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
    {
        // Arrange
        int nonExistentEventNoteId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _deleteHandler.Handle(new DeleteEventNoteCommand(nonExistentEventNoteId), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_Handler_DeletesOneNote_LeavesOthers()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var eventId = CreateTestEvent(dealId);
        var eventNoteId1 = CreateTestEventNote(eventId, "Note 1", 1);
        var eventNoteId2 = CreateTestEventNote(eventId, "Note 2", 2);
        var eventNoteId3 = CreateTestEventNote(eventId, "Note 3", 3);

        // Act - Delete the second note
        await _deleteHandler.Handle(new DeleteEventNoteCommand(eventNoteId2), CancellationToken.None);

        // Assert
        var remainingNotes = await DbContext.EventNotes
            .Where(en => en.EventId == eventId)
            .OrderBy(en => en.Order)
            .ToListAsync();

        remainingNotes.Should().HaveCount(2);
        remainingNotes[0].Text.Should().Be("Note 1");
        remainingNotes[1].Text.Should().Be("Note 3");

        // Verify the deleted note is gone
        var deletedNote = await DbContext.EventNotes.FirstOrDefaultAsync(en => en.Id == eventNoteId2);
        deletedNote.Should().BeNull();
    }

    #region Helpers
    private int CreateTestDeal()
    {
        var now = DateTime.UtcNow;
        var dealGuid = Guid.NewGuid().ToString();

        var deal = new Deal
        {
            Name = "Test Deal " + dealGuid,
            Description = "Test deal for event note tests",
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }

    private int CreateTestEvent(int dealId)
    {
        var now = DateTime.UtcNow;

        var evt = new Event
        {
            DealId = dealId,
            Date = DateTimeOffset.UtcNow,
            Topic = "Test event topic",
            Pos = 1,
            Agenda = "Test event",
            Result = "Test result",
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
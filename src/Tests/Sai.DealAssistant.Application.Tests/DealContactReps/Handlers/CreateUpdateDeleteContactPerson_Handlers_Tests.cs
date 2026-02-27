using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.ContactPersons.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers;

public class CreateUpdateDeleteContactPerson_Handlers_Tests : UnitTestBase
{
    private readonly CrudRepository<Infrastructure.Persistence.AppDbContext, ContactPerson> _repository;
    private readonly CreateContactPersonCommand.Handler _createHandler;
    private readonly UpdateContactPersonCommand.Handler _updateHandler;
    private readonly DeleteContactPersonCommand.Handler _deleteHandler;

    public CreateUpdateDeleteContactPerson_Handlers_Tests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<Infrastructure.Persistence.AppDbContext, ContactPerson>(DbContext);
        _createHandler = new CreateContactPersonCommand.Handler(_repository, Mapper);
        _updateHandler = new UpdateContactPersonCommand.Handler(_repository, Mapper);
        _deleteHandler = new DeleteContactPersonCommand.Handler(_repository, Mapper);
    }

    [Fact]
    public async Task Create_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();

        var cmd = new CreateContactPersonCommand
        {
            DealId = dealId,
            Name = "New Rep",
            Email = "new@example.com",
            Phone = "+1-555-0100",
            Position = "Sales Manager",
            Description = "New contact person"
        };

        // Act
        var result = await _createHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Rep");
        result.Email.Should().Be("new@example.com");
        result.Phone.Should().Be("+1-555-0100");
        result.Position.Should().Be("Sales Manager");
        result.Description.Should().Be("New contact person");

        // Verify in database
        var createdContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == result.Id);
        createdContactPerson.Should().NotBeNull();
        createdContactPerson!.Name.Should().Be("New Rep");
        createdContactPerson.Email.Should().Be("new@example.com");
        createdContactPerson.DealId.Should().Be(dealId);
    }

    [Fact]
    public async Task Create_Handler_CreatesMinimalContactPerson_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();

        var cmd = new CreateContactPersonCommand
        {
            DealId = dealId,
            Name = "Minimal Contact"
        };

        // Act
        var result = await _createHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Minimal Contact");

        // Verify in database
        var createdContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == result.Id);
        createdContactPerson.Should().NotBeNull();
        createdContactPerson!.Name.Should().Be("Minimal Contact");
        createdContactPerson.DealId.Should().Be(dealId);
    }

    [Fact]
    public async Task Update_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var contactPersonId = CreateTestContactPerson(dealId, "Original Name", "original@example.com");

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateContactPersonCommand
        {
            Id = contactPersonId,
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "+1-555-9999",
            Position = "Updated Position",
            Description = "Updated description"
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactPersonId);
        result.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@example.com");
        result.Phone.Should().Be("+1-555-9999");
        result.Position.Should().Be("Updated Position");

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == contactPersonId);
        updatedContactPerson.Should().NotBeNull();
        updatedContactPerson!.Name.Should().Be("Updated Name");
        updatedContactPerson.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task Update_Handler_ThrowsNotFound_WhenUpdateReturnsNull()
    {
        // Arrange
        var dealId = CreateTestDeal();
        int nonExistentContactPersonId = 999999;

        var cmd = new UpdateContactPersonCommand
        {
            Id = nonExistentContactPersonId,
            Name = "Does Not Exist"
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
        var contactPersonId = CreateTestContactPerson(dealId, "Original Name", "original@example.com");

        // Clear change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var cmd = new UpdateContactPersonCommand
        {
            Id = contactPersonId,
            Name = "Updated Name Only",
            Email = "original@example.com", // Keep original
            Phone = "+1-555-1111" // Update phone
        };

        // Act
        var result = await _updateHandler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name Only");
        result.Email.Should().Be("original@example.com");
        result.Phone.Should().Be("+1-555-1111");

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == contactPersonId);
        updatedContactPerson!.Name.Should().Be("Updated Name Only");
        updatedContactPerson.Email.Should().Be("original@example.com");
    }

    [Fact]
    public async Task Delete_Handler_ReturnsDto_OnSuccess()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var contactPersonId = CreateTestContactPerson(dealId, "To Delete", "delete@example.com");

        // Act
        var result = await _deleteHandler.Handle(new DeleteContactPersonCommand(contactPersonId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactPersonId);
        result.Name.Should().Be("To Delete");
        result.Email.Should().Be("delete@example.com");

        // Verify deleted from database
        var deletedContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == contactPersonId);
        deletedContactPerson.Should().BeNull();
    }

    [Fact]
    public async Task Delete_Handler_ThrowsNotFound_WhenDeleteReturnsNull()
    {
        // Arrange
        int nonExistentContactPersonId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _deleteHandler.Handle(new DeleteContactPersonCommand(nonExistentContactPersonId), CancellationToken.None));
    }

    [Fact]
    public async Task Delete_Handler_DeletesContactPersonWithEvents_Successfully()
    {
        // Arrange
        var dealId = CreateTestDeal();
        var contactPersonId = CreateTestContactPerson(dealId, "Contact with Events", "events@example.com");
        
        // Create an event associated with this contact person
        CreateTestEventForContactPerson(dealId, contactPersonId);

        // Act
        var result = await _deleteHandler.Handle(new DeleteContactPersonCommand(contactPersonId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactPersonId);

        // Verify deleted from database
        var deletedContactPerson = await DbContext.ContactPersons.FirstOrDefaultAsync(cp => cp.Id == contactPersonId);
        deletedContactPerson.Should().BeNull();

        // Verify events still exist (ContactPersonId should be null, not cascade deleted)
        var events = await DbContext.Events.Where(e => e.DealId == dealId).ToListAsync();
        events.Should().NotBeEmpty();
        events.All(e => e.ContactPersonId == null || e.ContactPersonId != contactPersonId).Should().BeTrue();
    }

    #region Helpers
    private int CreateTestDeal()
    {
        var now = DateTime.UtcNow;
        var dealGuid = Guid.NewGuid().ToString();

        var deal = new Deal
        {
            Name = "Test Deal " + dealGuid,
            Description = "Test deal for contact person tests",
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }

    private int CreateTestContactPerson(int dealId, string name, string email)
    {
        var now = DateTime.UtcNow;

        var contactPerson = new ContactPerson
        {
            DealId = dealId,
            Name = name,
            Email = email,
            Phone = "+1-555-0100",
            Position = "Test Position",
            Description = "Test contact person",
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.ContactPersons.Add(contactPerson);
        DbContext.SaveChanges();

        return contactPerson.Id;
    }

    private int CreateTestEventForContactPerson(int dealId, int contactPersonId)
    {
        var now = DateTime.UtcNow;

        var evt = new Event
        {
            DealId = dealId,
            ContactPersonId = contactPersonId,
            Date = DateTimeOffset.UtcNow,
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
    #endregion
}
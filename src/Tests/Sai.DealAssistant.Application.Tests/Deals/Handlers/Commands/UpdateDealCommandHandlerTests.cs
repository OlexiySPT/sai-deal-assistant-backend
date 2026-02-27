using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Commands;

public class UpdateDealCommandHandlerTests : UnitTestBase
{
    private readonly CrudRepository<Infrastructure.Persistence.AppDbContext, Deal> _repository;
    private readonly UpdateDealCommand.Handler _handler;

    public UpdateDealCommandHandlerTests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<Infrastructure.Persistence.AppDbContext, Deal>(DbContext);
        _handler = new UpdateDealCommand.Handler(_repository, Mapper);
    }

    [Fact]
    public async Task Handle_WhenRepositoryUpdates_ReturnsMappedDto()
    {
        // Arrange
        var dealId = CreateTestDeal();
        
        // Clear the change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var command = new UpdateDealCommand
        {
            Id = dealId,
            Name = "Updated Deal Name",
            Description = "Updated description",
            TypeId = 1,
            StateId = 1,
            Industry = "Updated Industry",
            Status = "Updated Status"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(dealId);
        result.Name.Should().Be("Updated Deal Name");
        result.Description.Should().Be("Updated description");
        result.Industry.Should().Be("Updated Industry");
        result.Status.Should().Be("Updated Status");

        // Verify deal was actually updated in database
        DbContext.ChangeTracker.Clear();
        var updatedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        updatedDeal.Should().NotBeNull();
        updatedDeal!.Name.Should().Be("Updated Deal Name");
        updatedDeal.Description.Should().Be("Updated description");
        updatedDeal.Industry.Should().Be("Updated Industry");
        updatedDeal.Status.Should().Be("Updated Status");
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        int nonExistentDealId = 999999;
        var command = new UpdateDealCommand
        {
            Id = nonExistentDealId,
            Name = "DoesNotExist",
            TypeId = 1,
            StateId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUpdatingPartialProperties_UpdatesOnlySpecifiedFields()
    {
        // Arrange
        var dealId = CreateTestDeal("Original Name", "Original Description", "Original Industry");
        
        // Clear the change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var command = new UpdateDealCommand
        {
            Id = dealId,
            Name = "New Name",
            Description = "Original Description", // Keep original
            TypeId = 1,
            StateId = 1,
            Industry = "New Industry",
            Status = "Active"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("Original Description");
        result.Industry.Should().Be("New Industry");

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        updatedDeal!.Name.Should().Be("New Name");
        updatedDeal.Description.Should().Be("Original Description");
        updatedDeal.Industry.Should().Be("New Industry");
    }

    [Fact]
    public async Task Handle_WhenUpdatingUrl_UpdatesSuccessfully()
    {
        // Arrange
        var dealId = CreateTestDeal();
        
        // Clear the change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var command = new UpdateDealCommand
        {
            Id = dealId,
            Name = "Deal with URL",
            Url = "https://example.com/updated-deal",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be("https://example.com/updated-deal");

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        updatedDeal!.Url.Should().Be("https://example.com/updated-deal");
    }

    [Fact]
    public async Task Handle_WhenUpdatingAllProperties_UpdatesSuccessfully()
    {
        // Arrange
        var dealId = CreateTestDeal();
        
        // Clear the change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var command = new UpdateDealCommand
        {
            Id = dealId,
            Name = "Fully Updated Deal",
            Description = "Fully updated description",
            Url = "https://example.com/fully-updated",
            AiSearchInfo = "Updated AI search info",
            AiBriefDescription = "Updated AI brief description",
            Industry = "Healthcare",
            Status = "Completed",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(dealId);
        result.Name.Should().Be("Fully Updated Deal");
        result.Description.Should().Be("Fully updated description");
        result.Url.Should().Be("https://example.com/fully-updated");
        result.AiSearchInfo.Should().Be("Updated AI search info");
        result.AiBriefDescription.Should().Be("Updated AI brief description");
        result.Industry.Should().Be("Healthcare");
        result.Status.Should().Be("Completed");

        // Verify all properties in database
        DbContext.ChangeTracker.Clear();
        var updatedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        updatedDeal.Should().NotBeNull();
        updatedDeal!.Name.Should().Be("Fully Updated Deal");
        updatedDeal.AiSearchInfo.Should().Be("Updated AI search info");
        updatedDeal.AiBriefDescription.Should().Be("Updated AI brief description");
        updatedDeal.Industry.Should().Be("Healthcare");
        updatedDeal.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task Handle_WhenClearingOptionalFields_UpdatesSuccessfully()
    {
        // Arrange
        var dealId = CreateTestDeal("Deal Name", "Description", "Industry");
        
        // Clear the change tracker to avoid tracking conflicts
        DbContext.ChangeTracker.Clear();

        var command = new UpdateDealCommand
        {
            Id = dealId,
            Name = "Deal Name",
            Description = null, // Clear description
            Industry = null,    // Clear industry
            Status = null,      // Clear status
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().BeNullOrEmpty();
        result.Industry.Should().BeNullOrEmpty();
        result.Status.Should().BeNullOrEmpty();

        // Verify in database
        DbContext.ChangeTracker.Clear();
        var updatedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        updatedDeal.Should().NotBeNull();
        updatedDeal!.Description.Should().BeNullOrEmpty();
        updatedDeal.Industry.Should().BeNullOrEmpty();
        updatedDeal.Status.Should().BeNullOrEmpty();
    }

    #region Helpers
    private int CreateTestDeal(string? name = null, string? description = null, string? industry = null)
    {
        var now = DateTime.UtcNow;
        var dealGuid = Guid.NewGuid().ToString();

        var deal = new Deal
        {
            Name = name ?? "Test Deal " + dealGuid,
            Description = description ?? "Test deal for update",
            Industry = industry ?? "Technology",
            Status = "Active",
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }
    #endregion
}
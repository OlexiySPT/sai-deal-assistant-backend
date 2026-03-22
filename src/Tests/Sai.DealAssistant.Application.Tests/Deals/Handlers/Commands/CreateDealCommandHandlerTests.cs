using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Commands;

public class CreateDealCommandHandlerTests : UnitTestBase
{
    private readonly CrudRepository<Infrastructure.Persistence.AppDbContext, Deal> _repository;
    private readonly CreateDealCommand.Handler _handler;

    public CreateDealCommandHandlerTests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<Infrastructure.Persistence.AppDbContext, Deal>(DbContext);
        _handler = new CreateDealCommand.Handler(_repository, Mapper);
    }

    [Fact]
    public async Task Handle_WhenRepositoryCreates_ReturnsMappedDto()
    {
        // Arrange
        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Test Deal " + Guid.NewGuid().ToString(),
            Company = "Test Company", // <-- Add this line
            Description = "Test deal description",
            TypeId = 1,
            StateId = 1,
            Industry = "Technology",
            Status = "Active"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(command.Name);
        result.Company.Should().Be(command.Company); // <-- Add this assertion
        result.Description.Should().Be(command.Description);
        result.TypeId.Should().Be(command.TypeId);
        result.StateId.Should().Be(command.StateId);
        result.Industry.Should().Be(command.Industry);
        result.Status.Should().Be(command.Status);

        // Verify deal was actually created in database
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.Name.Should().Be(command.Name);
        createdDeal.Company.Should().Be(command.Company); // <-- Add this assertion
        createdDeal.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_WhenCreatingDealWithUrl_ReturnsMappedDto()
    {
        // Arrange
        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Test Deal with URL " + Guid.NewGuid().ToString(),
            Company = "Test Company", // <-- Add this line
            Description = "Test deal with URL",
            Url = "https://example.com/deal",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Url.Should().Be(command.Url);

        // Verify deal was created with URL
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.Url.Should().Be(command.Url);
    }

    [Fact]
    public async Task Handle_WhenCreatingDealWithAllProperties_ReturnsMappedDto()
    {
        // Arrange
        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Comprehensive Test Deal " + Guid.NewGuid().ToString(),
            Company = "Comprehensive Test Company",
            Description = "Comprehensive test deal",
            Url = "https://example.com/comprehensive-deal",
            AiSearchInfo = "AI search information",
            AiBriefDescription = "AI brief description",
            Industry = "Finance",
            Status = "In Progress",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.Url.Should().Be(command.Url);
        result.AiSearchInfo.Should().Be(command.AiSearchInfo);
        result.AiBriefDescription.Should().Be(command.AiBriefDescription);
        result.Industry.Should().Be(command.Industry);
        result.Status.Should().Be(command.Status);

        // Verify all properties in database
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.AiSearchInfo.Should().Be(command.AiSearchInfo);
        createdDeal.AiBriefDescription.Should().Be(command.AiBriefDescription);
        createdDeal.Industry.Should().Be(command.Industry);
        createdDeal.Status.Should().Be(command.Status);
    }

    [Fact]
    public async Task Handle_WhenCreatingMinimalDeal_ReturnsMappedDto()
    {
        // Arrange
        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Minimal Deal " + Guid.NewGuid().ToString(),
            Company = "Minimal Test Company",
            TypeId = 1,
            StateId = 1
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(command.Name);
        result.TypeId.Should().Be(1);
        result.StateId.Should().Be(1);

        // Verify minimal deal was created
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.Name.Should().Be(command.Name);
    }
}
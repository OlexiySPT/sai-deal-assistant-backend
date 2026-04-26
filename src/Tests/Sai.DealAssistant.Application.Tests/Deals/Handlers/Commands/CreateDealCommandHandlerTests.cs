using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Commands;

public class CreateDealCommandHandlerTests : UnitTestBase
{
    private readonly CrudRepository<AppDbContext, Deal> _repository;
    private readonly CrudRepository<AppDbContext, Firm> _firmRepository;
    private readonly int _fidmId;

    private readonly CreateDealCommand.Handler _handler;

    public CreateDealCommandHandlerTests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<AppDbContext, Deal>(DbContext);
        _firmRepository = new CrudRepository<AppDbContext, Firm>(DbContext);
        _fidmId = DbContext.Firms.Select(f => f.Id).FirstOrDefault();

        // Use real repositories for handler
        _handler = new CreateDealCommand.Handler(
            _repository,
            Mapper,
            new Infrastructure.Repositories.FullFirmRepository(DbContext),
            _firmRepository,
            new Sai.DealAssistant.Infrastructure.UnitOfWork(DbContext)
        );
    }

    [Fact]
    public async Task Handle_WhenFirmIdIsLessThan1AndFirmNameProvided_CreatesFirmAndUsesItsId()
    {
        // Arrange
        var firmName = "New Firm " + Guid.NewGuid();
        var firmId = 0;

        // Use real handler with real repositories (DbContext-backed)
        var handler = _handler;

        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Deal with new firm " + Guid.NewGuid(),
            TypeId = 1,
            StateId = 1,
            FirmId = firmId,
            FirmName = firmName
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        // Verify firm was created in database
        var createdFirm = await DbContext.Firms.FirstOrDefaultAsync(f => f.Name == firmName);
        createdFirm.Should().NotBeNull();
        createdFirm!.Country.Should().Be("Unknown");
        result.FirmId.Should().Be(createdFirm.Id);

        // Verify deal was created and linked to the firm
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.FirmId.Should().Be(createdFirm.Id);
    }

    [Fact]
    public async Task Handle_WhenRepositoryCreates_ReturnsMappedDto()
    {
        // Arrange
        var command = new CreateDealCommand
        {
            StartDate = new DateOnly(2024, 1, 1),
            Name = "Test Deal " + Guid.NewGuid().ToString(),
            Description = "Test deal description",
            TypeId = 1,
            StateId = 1,
            FirmId = _fidmId, 
            Industry = "Technology",
            Status = "Active"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.TypeId.Should().Be(command.TypeId);
        result.StateId.Should().Be(command.StateId);
        result.Industry.Should().Be(command.Industry);
        result.Status.Should().Be(command.Status);

        // Verify deal was actually created in database
        var createdDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == result.Id);
        createdDeal.Should().NotBeNull();
        createdDeal!.Name.Should().Be(command.Name);
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
            Description = "Test deal with URL",
            Url = "https://example.com/deal",
            TypeId = 1,
            StateId = 1,
            FirmId  = _fidmId,
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
            Description = "Comprehensive test deal",
            Url = "https://example.com/comprehensive-deal",
            AiSearchInfo = "AI search information",
            AiBriefDescription = "AI brief description",
            Industry = "Finance",
            Status = "In Progress",
            TypeId = 1,
            StateId = 1, 
            FirmId = _fidmId,
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
            TypeId = 1,
            StateId = 1,
            FirmId = _fidmId,
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
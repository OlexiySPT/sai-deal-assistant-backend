using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Deals.Commands;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Commands;

public class DeleteDealCommandHandlerTests : UnitTestBase
{
    private readonly CrudRepository<Infrastructure.Persistence.AppDbContext, Deal> _repository;
    private readonly DeleteDealCommand.Handler _handler;
    private readonly int _testFirmId;

    public DeleteDealCommandHandlerTests()
        : base(seedTestData: true)
    {
        _repository = new CrudRepository<Infrastructure.Persistence.AppDbContext, Deal>(DbContext);
        _handler = new DeleteDealCommand.Handler(_repository, Mapper);
        _testFirmId = DbContext.Firms.Select(f => f.Id).FirstOrDefault();

    }

    [Fact]
    public async Task Handle_WhenRepositoryDeletes_ReturnsMappedDto()
    {
        // Arrange
        var dealId = CreateTestDeal();

        // Act
        var result = await _handler.Handle(new DeleteDealCommand(dealId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(dealId);
        result.Name.Should().StartWith("Test Deal");
        
        // Verify deal was actually deleted from database
        var deletedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        deletedDeal.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenDeleteReturnsNull_ThrowsNotFoundException()
    {
        // Arrange
        int nonExistentDealId = 999999;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => 
            _handler.Handle(new DeleteDealCommand(nonExistentDealId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenDealHasNavigationProperties_DeletesSuccessfullyWithCascade()
    {
        // Arrange
        var dealId = CreateTestDealWithNavigationProps();
        
        // Verify the deal has navigation properties before deletion
        var dealBeforeDelete = await DbContext.Deals
            .Include(d => d.Events)
                .ThenInclude(e => e.Notes)
            .Include(d => d.Tags)
            .FirstOrDefaultAsync(d => d.Id == dealId);

        dealBeforeDelete.Should().NotBeNull();
        dealBeforeDelete.Events.Should().HaveCount(2);
        dealBeforeDelete.Tags.Should().HaveCount(3);
        
        var eventIds = dealBeforeDelete.Events.Select(e => e.Id).ToList();
        var eventNoteIds = dealBeforeDelete.Events.SelectMany(e => e.Notes).Select(n => n.Id).ToList();
        var tagIds = dealBeforeDelete.Tags.Select(t => t.Id).ToList();

        // Act
        var result = await _handler.Handle(new DeleteDealCommand(dealId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(dealId);

        // Verify cascade delete worked - all related entities should be deleted
        var deletedDeal = await DbContext.Deals.FirstOrDefaultAsync(d => d.Id == dealId);
        deletedDeal.Should().BeNull();

        foreach (var eventId in eventIds)
        {
            var evt = await DbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            evt.Should().BeNull("Events should be cascade deleted");
        }

        foreach (var noteId in eventNoteIds)
        {
            var note = await DbContext.EventNotes.FirstOrDefaultAsync(n => n.Id == noteId);
            note.Should().BeNull("EventNotes should be cascade deleted");
        }

        foreach (var tagId in tagIds)
        {
            var tag = await DbContext.DealTags.FirstOrDefaultAsync(t => t.Id == tagId);
            tag.Should().BeNull("DealTags should be cascade deleted");
        }
    }

    #region Helpers
    private int CreateTestDeal()
    {
        var now = DateTime.UtcNow;
        var dealGuid = Guid.NewGuid().ToString();

        var deal = new Deal
        {
            Name = "Test Deal " + dealGuid,
            Description = "Test deal for deletion",
            TypeId = 1,
            StateId = 1,
            CreatedAt = now,
            UpdatedAt = now,
            FirmId = _testFirmId,
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }

    private int CreateTestDealWithNavigationProps()
    {
        var now = DateTime.UtcNow;
        var dealGuid = Guid.NewGuid().ToString();

        var deal = new Deal
        {
            Name = "Test Deal " + dealGuid,
            Description = "Comprehensive test deal with all navigation properties",
            Url = "https://example.com/deal/" + dealGuid,
            AiSearchInfo = "AI generated search information for testing",
            AiBriefDescription = "AI brief description of the test deal",
            Industry = "Technology",
            Status = "Active",
            TypeId = 1,
            StateId = 1,
            FirmId = _testFirmId,
            CreatedAt = now,
            UpdatedAt = now,
            Events = new List<Event>
            {
                new Event
                {
                    Date = DateTimeOffset.UtcNow.AddDays(-7),
                    Pos = 1,
                    Agenda = "Initial discovery call",
                    Topic = "Discuss client's needs and challenges",
                    Result = "Positive response, scheduled follow-up",
                    TypeId = 1,
                    StateId = 1,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Notes = new List<EventNote>
                    {
                        new EventNote
                        {
                            Order = 1,
                            Text = "Client expressed interest in our solution"
                        },
                        new EventNote
                        {
                            Order = 2,
                            Text = "Discussed budget constraints"
                        }
                    }
                },
                new Event
                {
                    Date = DateTimeOffset.UtcNow.AddDays(-3),
                    Pos = 2,
                    Agenda = "Technical demo",
                    Topic = "Showcase platform capabilities",
                    Result = "Client impressed with features",
                    TypeId = 1,
                    StateId = 1,
                    CreatedAt = now,
                    UpdatedAt = now,
                    Notes = new List<EventNote>
                    {
                        new EventNote
                        {
                            Order = 1,
                            Text = "Demonstrated core platform capabilities"
                        },
                        new EventNote
                        {
                            Order = 2,
                            Text = "Q&A session went well"
                        },
                        new EventNote
                        {
                            Order = 3,
                            Text = "Next step: proposal preparation"
                        }
                    }
                }
            },
            Tags = new List<DealTag>
            {
                new DealTag
                {
                    Tag = "Enterprise"
                },
                new DealTag
                {
                    Tag = "High Priority"
                },
                new DealTag
                {
                    Tag = "Q1-2026"
                }
            }
        };

        DbContext.Deals.Add(deal);
        DbContext.SaveChanges();

        return deal.Id;
    }
    #endregion
}
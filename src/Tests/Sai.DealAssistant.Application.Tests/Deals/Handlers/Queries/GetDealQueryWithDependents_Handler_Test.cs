using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Deals.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Repositories;
using SAI.DealAssistant.TestUtils.Unit;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Queries
{
    public class GetDealQueryWithDependents_Handler_Test : UnitTestBase
    {
        private readonly FullDealRepository _dealRepository;

        public GetDealQueryWithDependents_Handler_Test()
            : base(seedTestData: true)
        {
            _dealRepository = new FullDealRepository(DbContext);
        }

        [Fact]
        public async Task Handler_ReturnsDealDto_WhenDealExists()
        {
            // Arrange
            var handler = new GetDealWithDependentsQuery.Handler(_dealRepository, Mapper);

            // ContactPersons now live on Firm, not on Deal directly.
            // FullDealRepository includes d.Firm.ThenInclude(f => f.ContactPersons).
            var expectedEntity = DbContext.Deals
                .Include(d => d.Type)
                .Include(d => d.State)
                .Include(d => d.Firm).ThenInclude(f => f.ContactPersons)
                .Include(d => d.Events).ThenInclude(e => e.Notes)
                .Include(d => d.Tags)
                .AsNoTracking()
                .OrderByDescending(d => d.Id)
                .First();

            var query = new GetDealWithDependentsQuery(expectedEntity.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: basic scalar fields
            Assert.NotNull(result);
            Assert.Equal(expectedEntity.Id, result.Id);
            Assert.Equal(expectedEntity.Name, result.Name);
            Assert.Equal(expectedEntity.Description, result.Description);
            Assert.Equal(expectedEntity.Industry, result.Industry);

            // Assert: navigation DTOs exist and match counts/ids
            // TypeId and StateId are value types — no Assert.NotNull needed
            Assert.Equal(expectedEntity.TypeId, result.TypeId);
            Assert.Equal(expectedEntity.StateId, result.StateId);

            // ContactPersons come from the Deal's Firm, not from the Deal itself
            var expectedContactPersonCount = expectedEntity.Firm?.ContactPersons?.Count ?? 0;
            Assert.Equal(expectedContactPersonCount, result.Firm?.ContactPersons?.Count ?? 0);
            Assert.Equal(expectedEntity.Events?.Count ?? 0, result.Events?.Count ?? 0);
            Assert.Equal(expectedEntity.Tags?.Count ?? 0, result.Tags?.Count ?? 0);

            // If events exist, verify each event's notes count matches
            var expectedEventsOrdered = (expectedEntity.Events ?? Enumerable.Empty<Event>()).OrderBy(e => e.Id).ToList();
            var resultEventsOrdered = (result.Events ?? Enumerable.Empty<dynamic>()).OrderBy(e => e.Id).ToList();

            for (var i = 0; i < expectedEventsOrdered.Count; i++)
            {
                var expectedEvent = expectedEventsOrdered[i];
                var resultEvent = resultEventsOrdered[i];

                Assert.Equal(expectedEvent.Id, resultEvent.Id);
                Assert.Equal(expectedEvent.Notes?.Count ?? 0, (resultEvent.Notes as ICollection)?.Count ?? 0);
            }
        }

        [Fact]
        public async Task Handler_ThrowsNotFoundException_WhenDealDoesNotExist()
        {
            // Arrange
            var handler = new GetDealWithDependentsQuery.Handler(_dealRepository, Mapper);

            var nonExistentId = -9999;
            var query = new GetDealWithDependentsQuery(nonExistentId);

            // Act / Assert
            await Assert.ThrowsAsync<NotFoundExceptionOverride>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_ReturnsDealWithEmptyNavigations_WhenNoDependents()
        {
            // Arrange
            var handler = new GetDealWithDependentsQuery.Handler(_dealRepository, Mapper);

            // A deal with no firm has no contact persons
            var typeId = DbContext.DealTypes.AsNoTracking().Select(t => t.Id).First();
            var stateId = DbContext.DealStates.AsNoTracking().Select(s => s.Id).First();

            var firmId = DbContext.Firms.AsNoTracking().Select(f => f.Id).First();

            var minimalDeal = new Deal
            {
                Name = "MinimalDeal_For_Test",
                Description = "Minimal",
                Industry = "Test",
                TypeId = typeId,
                StateId = stateId,
                FirmId = firmId
            };

            DbContext.Deals.Add(minimalDeal);
            DbContext.SaveChanges();

            var query = new GetDealWithDependentsQuery(minimalDeal.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(minimalDeal.Id, result.Id);
            Assert.Equal(minimalDeal.Name, result.Name);

            // Firm may or may not have contact persons; confirm only events and tags are empty
            Assert.Equal(0, result.Events?.Count ?? 0);
            Assert.Equal(0, result.Events?.Count ?? 0);
            Assert.Equal(0, result.Tags?.Count ?? 0);
        }

        [Fact]
        public async Task Handler_IncludesEventNotes_WhenEventsHaveNotes()
        {
            // Arrange
            var handler = new GetDealWithDependentsQuery.Handler(_dealRepository, Mapper);

            // Try to find an existing deal with events and notes seeded; if none exist, create one
            var dealWithEvents = DbContext.Deals
                .Include(d => d.Events).ThenInclude(e => e.Notes)
                .FirstOrDefault(d => d.Events.Any(e => e.Notes.Any()));

            if (dealWithEvents == null)
            {
                var typeId = DbContext.DealTypes.AsNoTracking().Select(t => t.Id).First();
                var stateId = DbContext.DealStates.AsNoTracking().Select(s => s.Id).First();

                var seedFirmId = DbContext.Firms.AsNoTracking().Select(f => f.Id).First();

                var newDeal = new Deal
                {
                    Name = "Deal_With_Event_Notes",
                    Description = "Created for test",
                    Industry = "Test",
                    TypeId = typeId,
                    StateId = stateId,
                    FirmId = seedFirmId
                };
                DbContext.Deals.Add(newDeal);
                DbContext.SaveChanges();

                var evt = new Event
                {
                    DealId = newDeal.Id,
                    Topic = "Test Event Topic",
                    Agenda = "evt",
                    Date = DateTimeOffset.UtcNow,
                    TypeId = 1,
                    StateId = 1
                };
                DbContext.Events.Add(evt);
                DbContext.SaveChanges();

                var note = new EventNote
                {
                    EventId = evt.Id,
                    Text = "Test note"
                };
                DbContext.EventNotes.Add(note);
                DbContext.SaveChanges();

                dealWithEvents = DbContext.Deals
                    .Include(d => d.Events).ThenInclude(e => e.Notes)
                    .AsNoTracking()
                    .First(d => d.Id == newDeal.Id);
            }
            else
            {
                dealWithEvents = DbContext.Deals
                    .Include(d => d.Events).ThenInclude(e => e.Notes)
                    .AsNoTracking()
                    .First(d => d.Id == dealWithEvents.Id);
            }

            var query = new GetDealWithDependentsQuery(dealWithEvents.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: events and their notes are present and counts match
            Assert.NotNull(result);
            Assert.NotEmpty(result.Events);

            var expectedEventsOrdered = dealWithEvents.Events.OrderBy(e => e.Id).ToList();
            var resultEventsOrdered = result.Events.OrderBy(e => e.Id).ToList();

            for (var i = 0; i < expectedEventsOrdered.Count; i++)
            {
                var expectedEvent = expectedEventsOrdered[i];
                var resultEvent = resultEventsOrdered[i];

                Assert.Equal(expectedEvent.Id, resultEvent.Id);
                Assert.Equal(expectedEvent.Notes?.Count ?? 0, (resultEvent.Notes as ICollection)?.Count ?? 0);
            }
        }
    }
}
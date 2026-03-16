using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
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

			// load expected entity including navigations so we can compare navigation data
			var expectedEntity = DbContext.Deals
				.Include(d => d.Type)
				.Include(d => d.State)
				.Include(d => d.ContactPersons)
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
			Assert.NotNull(result.TypeId);
			Assert.Equal(expectedEntity.TypeId, result.TypeId);

			Assert.NotNull(result.StateId);
			Assert.Equal(expectedEntity.StateId, result.StateId);

			Assert.Equal(expectedEntity.ContactPersons?.Count ?? 0, result.ContactPersons?.Count ?? 0);
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

			// Use an id that is very unlikely to exist
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

			// create a minimal deal with no contact persons, events or tags
			var typeId = DbContext.DealTypes.AsNoTracking().Select(t => t.Id).First();
			var stateId = DbContext.DealStates.AsNoTracking().Select(s => s.Id).First();

			var minimalDeal = new Deal
			{
				Name = "MinimalDeal_For_Test",
				Description = "Minimal",
				Industry = "Test",
				TypeId = typeId,
				StateId = stateId
			};

			DbContext.Deals.Add(minimalDeal);
			DbContext.SaveChanges();

			// load expected entity without navigations (they should be empty)
			var expectedEntity = DbContext.Deals
				.AsNoTracking()
				.First(d => d.Id == minimalDeal.Id);

			var query = new GetDealWithDependentsQuery(expectedEntity.Id);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedEntity.Id, result.Id);
			Assert.Equal(expectedEntity.Name, result.Name);

			// navigation collections should be empty or null depending on mapping; assert counts are zero
			Assert.Equal(0, result.ContactPersons?.Count ?? 0);
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
				// create a deal and attach an event with a note
				var typeId = DbContext.DealTypes.AsNoTracking().Select(t => t.Id).First();
				var stateId = DbContext.DealStates.AsNoTracking().Select(s => s.Id).First();

				var newDeal = new Deal
				{
					Name = "Deal_With_Event_Notes",
					Description = "Created for test",
					Industry = "Test",
					TypeId = typeId,
					StateId = stateId
				};
				DbContext.Deals.Add(newDeal);
				DbContext.SaveChanges();

				var evt = new Event
				{
					DealId = newDeal.Id,
					Agenda = "evt",
					Date = DateTime.UtcNow
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
			Assert.True((dealWithEvents.Events?.Count ?? 0) > 0, "Test setup expected at least one event");
			Assert.Equal(dealWithEvents.Events.Count, result.Events?.Count ?? 0);

			var expectedEvent = dealWithEvents.Events.OrderBy(e => e.Id).First();
			var resultEvent = (result.Events ?? Enumerable.Empty<dynamic>()).OrderBy(e => e.Id).FirstOrDefault();

			Assert.NotNull(resultEvent);
			Assert.Equal(expectedEvent.Id, resultEvent.Id);
			Assert.Equal(expectedEvent.Notes?.Count ?? 0, (resultEvent.Notes as ICollection)?.Count ?? 0);
		}
	}
}
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.EventNotes.Queries;
using Sai.DealAssistant.Application.Entities.EventNotes.Dto;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;

namespace Tests.Sai.DealAssistant.Application.Tests.EventNotes.Handlers
{
    public class GetEventNotesQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, EventNote> _repo;

        public GetEventNotesQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, EventNote>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var state = db.DealStates.Add(new DealState { State = "Open" });
                var type = db.DealTypes.Add(new DealType { Type = "Standard" });
                var eventtype = db.EventTypes.Add(new EventType { Name = "Meeting" });
                var eventstate = db.EventStates.Add(new EventState { State = "Scheduled" });
                db.SaveChanges();

                var deal = new Deal { Name = "Deal A", StateId = state.Entity.Id, TypeId = type.Entity.Id };
                db.Add(deal);
                db.SaveChanges();

                var ev1 = new Event { Agenda = "Event A", DealId = deal.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id };
                var ev2 = new Event { Agenda = "Event B", DealId = deal.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id };
                db.AddRange(ev1, ev2);
                db.SaveChanges();

                var notes = new[]
                {
                    new EventNote { Text = "Note Z", Order = 2, EventId = ev1.Id },
                    new EventNote { Text = "Note A", Order = 1, EventId = ev1.Id },
                    new EventNote { Text = "Other", Order = 1, EventId = ev2.Id }
                };

                db.EventNotes.AddRange(notes);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsOnlyNotesForGivenEvent_OrderedByOrder()
        {
            // Arrange
            var handler = new GetEventNotesQuery.Handler(_repo);

            var eventEntity = DbContext.Events.Include(e => e.Notes).First();
            var query = new GetEventNotesQuery { EventId = eventEntity.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.EventNotes
                .Where(n => n.EventId == eventEntity.Id)
                .OrderBy(n => n.Order)
                .Select(n => new EventNoteListItemDto
                {
                    Id = n.Id,
                    Order = n.Order,
                    Text = n.Text
                })
                .ToList();

            Assert.Equal(expected.Count, result.TotalItems);
            Assert.Equal(expected.Count, result.Items.Count);
            Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
            Assert.Equal(expected.Select(x => x.Order), result.Items.Select(x => x.Order));
            Assert.Equal(expected.Select(x => x.Text), result.Items.Select(x => x.Text));
        }
    }
}
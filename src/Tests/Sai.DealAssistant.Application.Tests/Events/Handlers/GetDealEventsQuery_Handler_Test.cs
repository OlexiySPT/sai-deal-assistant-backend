using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.Events.Dtos;
using Sai.DealAssistant.Application.Entities.Events.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Events.Handlers
{
    public class GetDealEventsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, Event> _repo;

        public GetDealEventsQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, Event>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var dealtype = db.DealTypes.Add(new DealType { Type = "Standard" });
                var dealstate = db.DealStates.Add(new DealState { State = "Open" });
                var at1 = db.AmountTypes.Add(new AmountType { Type = "Per Month" });
                var eventtype = db.EventTypes.Add(new EventType { Name = "Meeting" });
                var eventstate = db.EventStates.Add(new EventState { State = "Planned" });
                db.SaveChanges();

                // Firms own the ContactPersons; Deals reference the Firm via FirmId
                var firm1 = new Firm { Name = "Firm A", Country = "USA" };
                var firm2 = new Firm { Name = "Firm B", Country = "UK" };
                db.Firms.AddRange(firm1, firm2);
                db.SaveChanges();

                var cp1 = new ContactPerson { Name = "Kurt Tank", FirmId = firm1.Id };
                var cp2 = new ContactPerson { Name = "Leroy Grumman", FirmId = firm2.Id };
                db.ContactPersons.AddRange(cp1, cp2);
                db.SaveChanges();

                var deal1 = new Deal { Name = "Deal A", TypeId = dealtype.Entity.Id, StateId = dealstate.Entity.Id, AmountTypeId = at1.Entity.Id, FirmId = firm1.Id };
                var deal2 = new Deal { Name = "Deal B", TypeId = dealtype.Entity.Id, StateId = dealstate.Entity.Id, AmountTypeId = at1.Entity.Id, FirmId = firm2.Id };
                db.AddRange(deal1, deal2);
                db.SaveChanges();

                var events = new[]
                {
                    new Event { Topic = "Event 1", Date = DateTimeOffset.UtcNow.AddDays(-1), Agenda = "A1", DealId = deal1.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id, ContactPersonId = cp1.Id },
                    new Event { Topic = "Event 2", Date = DateTimeOffset.UtcNow.AddDays(-2), Agenda = "A2", DealId = deal1.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id, ContactPersonId = cp1.Id },
                    new Event { Topic = "Event 3", Date = DateTimeOffset.UtcNow,             Agenda = "B1", DealId = deal2.Id, TypeId = eventtype.Entity.Id, StateId = eventstate.Entity.Id, ContactPersonId = cp2.Id }
                };

                db.Events.AddRange(events);
                db.SaveChanges();
            }
        }

        [Fact]
        public async void Handler_ReturnsOnlyEventsForGivenDeal_OrderedByIdDesc()
        {
            // Arrange
            var handler = new GetDealEventsQuery.Handler(_repo);
            var deal = DbContext.Deals.Include(d => d.Events).First();
            var query = new GetDealEventsQuery { DealId = deal.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.Events
                .Where(p => p.DealId == deal.Id)
                .OrderByDescending(p => p.Id)
                .Select(p => new EventListItemDto
                {
                    Id = p.Id,
                    Topic = p.Topic,
                    Date = p.Date,
                    Pos = p.Pos,
                    Agenda = p.Agenda,
                    Result = p.Result,
                    State = p.State != null ? p.State.State : null!,
                    Type = p.Type != null ? p.Type.Name : null!,
                    ContactPerson = p.ContactPerson != null ? $"{p.ContactPerson.Name}, {p.ContactPerson.Position}" : null
                })
                .ToList();

            Assert.Equal(expected.Count, result.TotalItems);
            Assert.Equal(expected.Count, result.Items.Count);
            Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
            Assert.Equal(expected.Select(x => x.ContactPerson), result.Items.Select(x => x.ContactPerson));
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
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
                var deal1 = new Deal { Name = "Deal A" };
                var deal2 = new Deal { Name = "Deal B" };
                db.AddRange(deal1, deal2);
                db.SaveChanges();

                var events = new[]
                {
                    new Event { Date = DateTimeOffset.UtcNow.AddDays(-1), Agenda = "A1", DealId = deal1.Id },
                    new Event { Date = DateTimeOffset.UtcNow.AddDays(-2), Agenda = "A2", DealId = deal1.Id },
                    new Event { Date = DateTimeOffset.UtcNow, Agenda = "B1", DealId = deal2.Id }
                };

                db.Events.AddRange(events);
                db.SaveChanges();
            }
        }

        [Fact]
        public async void Handler_ReturnsOnlyEventsForGivenDeal_OrderedByDateDesc()
        {
            // Arrange
            var handler = new Sai.DealAssistant.Application.Entities.Events.Queries.GetDealEventsQuery.Handler(_repo);
            var deal = DbContext.Deals.Include(d => d.Events).First();
            var query = new Sai.DealAssistant.Application.Entities.Events.Queries.GetDealEventsQuery { DealId = deal.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.Events
                .Where(p => p.DealId == deal.Id)
                .OrderByDescending(p => p.Date)
                .Select(p => new Sai.DealAssistant.Application.Entities.Events.Dtos.EventListItemDto
                {
                    Id = p.Id,
                    Date = p.Date,
                    Pos = p.Pos,
                    Agenda = p.Agenda,
                    Result = p.Result,
                    State = p.State != null ? p.State.State : null!,
                    Type = p.Type != null ? p.Type.Name : null!
                })
                .ToList();

            Assert.Equal(expected.Count, result.TotalItems);
            Assert.Equal(expected.Count, result.Items.Count);
            Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
            // ensure ordering by date desc
            Assert.Equal(expected.Select(x => x.Date), result.Items.Select(x => x.Date));
        }
    }
}
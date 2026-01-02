using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class GetDealContactRepsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, DealContactRep> _repo;

        public GetDealContactRepsQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, DealContactRep>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var deal1 = new Deal { Name = "Deal A" };
                var deal2 = new Deal { Name = "Deal B" };
                db.AddRange(deal1, deal2);
                db.SaveChanges();

                var reps = new[]
                {
                    new DealContactRep { Name = "Zoe", Email = "z@example.com", DealId = deal1.Id },
                    new DealContactRep { Name = "Alice", Email = "a@example.com", DealId = deal1.Id },
                    new DealContactRep { Name = "Bob", Email = "b@example.com", DealId = deal2.Id }
                };

                db.DealContactReps.AddRange(reps);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsOnlyRepsForGivenDeal_OrderedByName()
        {
            // Arrange
            var handler = new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepsQuery.Handler(_repo);

            var deal = DbContext.Deals.Include(d => d.ContactReps).First();
            var query = new Sai.DealAssistant.Application.Entities.DealContactReps.Queries.GetDealContactRepsQuery { DealId = deal.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.DealContactReps
                .Where(p => p.DealId == deal.Id)
                .OrderBy(p => p.Name)
                .Select(p => new Sai.DealAssistant.Application.Entities.DealContactReps.Dtos.DealContactRepListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Position = p.Position,
                    Email = p.Email
                })
                .ToList();

            Assert.Equal(expected.Count, result.TotalItems);
            Assert.Equal(expected.Count, result.Items.Count);
            Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
            Assert.Equal(expected.Select(x => x.Name), result.Items.Select(x => x.Name));
        }
    }
}
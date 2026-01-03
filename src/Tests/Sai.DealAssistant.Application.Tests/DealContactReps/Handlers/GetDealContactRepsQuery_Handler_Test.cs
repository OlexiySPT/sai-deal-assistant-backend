using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.DealContactReps.Dtos;
using Sai.DealAssistant.Application.Entities.DealContactReps.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class GetDealContactRepsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, ContactPerson> _repo;

        public GetDealContactRepsQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, ContactPerson>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var state = db.DealStates.Add(new DealState { State = "Open" });
                var type = db.DealTypes.Add(new DealType { Type = "Standard" });
                db.SaveChanges();
                var deal1 = new Deal { Name = "Deal A" , StateId = state.Entity.Id, TypeId = type.Entity.Id};
                var deal2 = new Deal { Name = "Deal B", StateId = state.Entity.Id, TypeId = type.Entity.Id };
                db.AddRange(deal1, deal2);
                db.SaveChanges();

                var reps = new[]
                {
                    new ContactPerson { Name = "Zoe", Email = "z@example.com", DealId = deal1.Id },
                    new ContactPerson { Name = "Alice", Email = "a@example.com", DealId = deal1.Id },
                    new ContactPerson { Name = "Bob", Email = "b@example.com", DealId = deal2.Id }
                };

                db.ContactPersons.AddRange(reps);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsOnlyRepsForGivenDeal_OrderedByName()
        {
            // Arrange
            var handler = new GetDealContactRepsQuery.Handler(_repo);

            var deal = DbContext.Deals.Include(d => d.ContactPersons).First();
            var query = new GetDealContactRepsQuery { DealId = deal.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.ContactPersons
                .Where(p => p.DealId == deal.Id)
                .OrderBy(p => p.Name)
                .Select(p => new DealContactRepListItemDto
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
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.ContactPersons.Dto;
using Sai.DealAssistant.Application.Entities.ContactPersons.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealContactReps.Handlers
{
    public class GetDealContactPersonsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, ContactPerson> _repo;

        public GetDealContactPersonsQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _repo = new ReadRepository<AppDbContext, ContactPerson>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var firm1 = new Firm { Name = "Firm A", Country = "USA" };
                var firm2 = new Firm { Name = "Firm B", Country = "UK" };
                db.Firms.AddRange(firm1, firm2);
                db.SaveChanges();

                var reps = new[]
                {
                    new ContactPerson { Name = "Zoe",   Email = "z@example.com", FirmId = firm1.Id },
                    new ContactPerson { Name = "Alice",  Email = "a@example.com", FirmId = firm1.Id },
                    new ContactPerson { Name = "Bob",    Email = "b@example.com", FirmId = firm2.Id }
                };

                db.ContactPersons.AddRange(reps);
                db.SaveChanges();
            }
        }

        [Fact]
        public async Task Handler_ReturnsOnlyRepsForGivenFirm_OrderedByName()
        {
            // Arrange
            var handler = new GetFirmContactPersonsQuery.Handler(_repo);

            var firm = DbContext.Firms.Include(f => f.ContactPersons).First();
            var query = new GetFirmContactPersonsQuery { FirmId = firm.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert - expected built from DbContext
            var expected = DbContext.ContactPersons
                .Where(p => p.FirmId == firm.Id)
                .OrderBy(p => p.Name)
                .Select(p => new ContactPersonListItemDto
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
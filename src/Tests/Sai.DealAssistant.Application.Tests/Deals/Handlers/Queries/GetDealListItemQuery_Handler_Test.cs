using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.Deals.Dtos;
using Sai.DealAssistant.Application.Entities.Deals.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Entities.ReadOnly.Enums;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Deals.Handlers.Queries
{
    public class GetDealListItemQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, Deal> _dealRepository;

        public GetDealListItemQuery_Handler_Test()
            : base(seedTestData: false)
        {
            _dealRepository = new ReadRepository<AppDbContext, Deal>(DbContext);

            // Seed test data
            using (var db = CreateNewDbContext())
            {
                var s = new DealState { State = "New" };
                var t = new DealType { Type = "Standard" };
                var at = new AmountType { Type = "Per Month" };
                var firm = new Firm { Name = "Test Firm", Country = "USA" };
                db.AddRange(s, t, at, firm);
                db.SaveChanges();

                var deals = Enumerable.Range(1, 3).Select(i => new Deal
                {
                    Name = $"Deal {i}",
                    Description = $"Desc {i}",
                    Industry = i % 2 == 0 ? "Software" : "Finance",
                    StateId = s.Id,
                    TypeId = t.Id,
                    AmountTypeId = at.Id,
                    FirmId = firm.Id
                }).ToArray();

                db.Deals.AddRange(deals);
                db.SaveChanges();
            }
        }

        [Fact]
        public async void Handler_ReturnsDealListItemDto_WhenDealExists()
        {
            // Arrange
            var handler = new GetDealListItemQuery.Handler(_dealRepository);
            var expectedEntity = DbContext.Deals
                .Include(d => d.State)
                .OrderBy(d => d.Id)
                .First();
            var query = new GetDealListItemQuery(expectedEntity.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntity.Id, result.Id);
            Assert.Equal(expectedEntity.Name, result.Name);
            Assert.Equal(expectedEntity.DenormFirmName, result.FirmName);
            Assert.Equal(expectedEntity.DenormLastActionDate, result.LastActionDate);
            Assert.Equal(expectedEntity.State.State, result.State);
            Assert.Equal(expectedEntity.Status, result.Status);
        }

        [Fact]
        public async void Handler_ReturnsNull_WhenDealDoesNotExist()
        {
            // Arrange
            var handler = new GetDealListItemQuery.Handler(_dealRepository);
            var nonExistingId = DbContext.Deals.OrderByDescending(d => d.Id).First().Id + 100;
            var query = new GetDealListItemQuery(nonExistingId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async void Handler_ReturnsCorrectDto_ForEachDeal()
        {
            // Arrange
            var handler = new GetDealListItemQuery.Handler(_dealRepository);
            var allDeals = DbContext.Deals
                .Include(d => d.State)
                .OrderBy(d => d.Id)
                .ToList();

            foreach (var expectedEntity in allDeals)
            {
                var query = new GetDealListItemQuery(expectedEntity.Id);

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                var expected = new DealListItemDto
                {
                    Id = expectedEntity.Id,
                    Name = expectedEntity.Name,
                    FirmName = expectedEntity.DenormFirmName,
                    LastActionDate = expectedEntity.DenormLastActionDate,
                    State = expectedEntity.State.State,
                    Status = expectedEntity.Status,
                };
                Assert.Equivalent(expected, result);
            }
        }
    }
}

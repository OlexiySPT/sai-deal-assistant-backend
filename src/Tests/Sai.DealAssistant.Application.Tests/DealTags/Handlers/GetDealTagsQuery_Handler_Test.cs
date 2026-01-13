using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application.Entities.DealTags.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.DealTags.Handlers
{
    public class GetDealTagsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, DealTag> _repo;

        public GetDealTagsQuery_Handler_Test()
            : base(seedTestData: true)
        {
            _repo = new ReadRepository<AppDbContext, DealTag>(DbContext);
        }

        [Fact]
        public async Task Handle_ReturnsTags_ForExistingDeal()
        {
            // Arrange
            var handler = new GetDealTagsQuery.Handler(_repo);
            var dealId = await _repo.GetAll().Select(p => p.DealId).FirstOrDefaultAsync();
            // Ensure test data seeded contains at least one tag for the selected deal
            var expectedCount = await _repo.GetAll().Where(p => p.DealId == dealId).CountAsync();
            Assert.True(expectedCount > 0, "Test requires seeded DealTag data.");

            // Act
            var result = (await handler.Handle(new GetDealTagsQuery { DealId = dealId }, CancellationToken.None)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count);
            Assert.All(result, r => Assert.Equal(dealId, r.DealId));

            var tags = result.Select(r => r.Tag).ToList();
            var sorted = tags.OrderBy(t => t).ToList();
            Assert.Equal(sorted, tags); // ensure ordering by Tag
        }

        [Fact]
        public async Task Handle_ReturnsEmpty_ForNonExistingDeal()
        {
            // Arrange
            var handler = new GetDealTagsQuery.Handler(_repo);
            var nonExistingDealId = -9999;

            // Act
            var result = await handler.Handle(new GetDealTagsQuery { DealId = nonExistingDealId }, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
    }
}
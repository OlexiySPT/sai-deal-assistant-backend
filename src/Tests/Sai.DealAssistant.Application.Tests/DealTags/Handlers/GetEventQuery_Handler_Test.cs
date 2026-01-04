using Microsoft.EntityFrameworkCore;
using Moq;
using Sai.DealAssistant.Application.Entities.DealTags.Queries;
using Sai.DealAssistant.Common.Configuration;
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
    public class GetCachedTagsQuery_Handler_Test : UnitTestBase
    {
        private readonly ReadRepository<AppDbContext, DealTag> _repo;
        private readonly Mock<IAppConfiguration> _config = (Mock<IAppConfiguration>)new Mock<IAppConfiguration>().SetupGet<int>(p => p.EnumTablesCacheExpitrationMins).Returns(10);


        public GetCachedTagsQuery_Handler_Test()
            : base(seedTestData: true)
        {
            _repo = new ReadRepository<AppDbContext, DealTag>(DbContext);
        }

        [Fact]
        public async Task Handler_ReturnsActualCachedTags()
        {
            // Arrange
            var existing = await DbContext.DealTags.Select(p => p.Tag).Distinct().OrderBy(p => p).ToArrayAsync();
            var handler = new GetCachedTagsQuery.Handler(_repo, _config.Object);

            // Act
            var result = await handler.Handle(new GetCachedTagsQuery(), CancellationToken.None);

            // Assert
            Assert.Equivalent(existing, result.OrderBy(p => p).ToArray());
        }

        [Fact]
        public async Task Handler_ReturnsCachedTags_DespiteTagsAlreadyUpdated()
        {
            // Arrange
            var handler = new GetCachedTagsQuery.Handler(_repo, _config.Object);
            const string tagStr = "yyyYYY New Tag";
            // Read and cache previous state
            var result1 = await handler.Handle(new GetCachedTagsQuery(), CancellationToken.None);
            //Added new tag
            var firstDeal = await DbContext.Deals.FirstAsync();
            DbContext.DealTags.Add(new DealTag { DealId = firstDeal.Id, Tag = tagStr });
            await DbContext.SaveChangesAsync();

            // Act
            var result2 = await handler.Handle(new GetCachedTagsQuery(), CancellationToken.None);

            // Assert
            Assert.DoesNotContain(result2, p => p == tagStr);
        }

        [Fact]
        public async Task Handler_ReturnsActualCachedTags_AfterUpdateAndInvalidation()
        {
            // Arrange
            var handler = new GetCachedTagsQuery.Handler(_repo, _config.Object);
            const string tagStr = "yyyYYY New Tag";
            // Read and cache previous state
            var result1 = await handler.Handle(new GetCachedTagsQuery(), CancellationToken.None);
            // Act
            //Added new tag
            var firstDeal = await DbContext.Deals.FirstAsync();
            DbContext.DealTags.Add(new DealTag { DealId = firstDeal.Id, Tag = tagStr });
            await DbContext.SaveChangesAsync();
            handler.InvalidateCache();
            var result2 = await handler.Handle(new GetCachedTagsQuery(), CancellationToken.None);
            //Assert
            Assert.DoesNotContain(result1, p => p == tagStr);
            Assert.Contains(result2, p => p == tagStr);
        }
    }
}
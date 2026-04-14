using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sai.DealAssistant.Application.Entities.General.Queries;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Domain.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.General.Queries
{
    public class GetCachedStringOptionsTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IReadRepository<Deal>> _dealRepoMock;
        private readonly Mock<IAppConfiguration> _configMock;

        public GetCachedStringOptionsTests()
        {
            var services = new ServiceCollection();
            _dealRepoMock = new Mock<IReadRepository<Deal>>();
            _configMock = new Mock<IAppConfiguration>();
            _configMock.SetupGet(x => x.EnumTablesCacheExpitrationMins).Returns(10);
            services.AddSingleton(_dealRepoMock.Object);
            services.AddSingleton(_configMock.Object);
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task ReturnsDistinctOptions_ForDealStatus()
        {
            // Arrange
            var statuses = new List<string> { "Open", "Closed", "Open", "Pending" };
            _dealRepoMock.Setup(r => r.GetAll()).Returns(new List<Deal> {
                new Deal { Status = "Open" },
                new Deal { Status = "Closed" },
                new Deal { Status = "Open" },
                new Deal { Status = "Pending" }
            }.AsQueryable());
            _dealRepoMock.Setup(r => r.SelectDistinctAsync(
                It.IsAny<IQueryable<Deal>>(),
                It.IsAny<global::System.Linq.Expressions.Expression<Func<Deal, string>>>(),
                It.IsAny<SortDirection?>()
            )).ReturnsAsync((IQueryable<Deal> q, global::System.Linq.Expressions.Expression<Func<Deal, string>> e, SortDirection? s) =>
                new HashSet<string>(statuses));

            var handler = new GetCachedStringOptions.Handler(_serviceProvider, _configMock.Object);
            var query = new GetCachedStringOptions
            {
                EntityType = typeof(Deal),
                FieldName = "Status",
                SortDirection = SortDirection.Ascending
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Contains("Open", result);
            Assert.Contains("Closed", result);
            Assert.Contains("Pending", result);
            Assert.Equal(3, new HashSet<string>(result).Count);
        }

        [Fact]
        public async Task ThrowsArgumentException_WhenFieldNameMissing()
        {
            var handler = new GetCachedStringOptions.Handler(_serviceProvider, _configMock.Object);
            var query = new GetCachedStringOptions
            {
                EntityType = typeof(Deal),
                FieldName = "",
                SortDirection = SortDirection.Ascending
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsArgumentException_WhenEntityTypeMissing()
        {
            var handler = new GetCachedStringOptions.Handler(_serviceProvider, _configMock.Object);
            var query = new GetCachedStringOptions
            {
                EntityType = null!,
                FieldName = "Status",
                SortDirection = SortDirection.Ascending
            };
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}

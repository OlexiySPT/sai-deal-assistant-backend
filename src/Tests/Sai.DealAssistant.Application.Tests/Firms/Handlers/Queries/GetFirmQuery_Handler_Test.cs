using AutoMapper;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Application.Entities.Firms.Queries;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Firms.Handlers.Queries
{
	public class GetFirmQuery_Handler_Test : UnitTestBase
	{
		private readonly ReadRepository<AppDbContext, Firm> _firmRepository;
		private readonly IMapper _mapper;

		public GetFirmQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_firmRepository = new ReadRepository<AppDbContext, Firm>(DbContext);

			var cfg = new MapperConfiguration(c => c.AddProfile<FirmDto.MappingProfile>(), LoggerFactory);
			_mapper = cfg.CreateMapper();

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				var firms = Enumerable.Range(1, 3).Select(i => new Firm
				{
					Name = $"Firm {i}",
					Country = i % 2 == 0 ? "Germany" : "USA",
					Description = $"Description {i}"
				}).ToArray();

				db.Firms.AddRange(firms);
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_ReturnsFirmDto_WhenFirmExists()
		{
			// Arrange
			var handler = new GetFirmQuery.Handler(_firmRepository, _mapper);
			var expectedEntity = DbContext.Firms.OrderBy(f => f.Id).First();
			var query = new GetFirmQuery(expectedEntity.Id);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedEntity.Id, result.Id);
			Assert.Equal(expectedEntity.Name, result.Name);
			Assert.Equal(expectedEntity.Country, result.Country);
			Assert.Equal(expectedEntity.Description, result.Description);
		}

		[Fact]
		public async void Handler_ThrowsNotFoundException_WhenFirmDoesNotExist()
		{
			// Arrange
			var handler = new GetFirmQuery.Handler(_firmRepository, _mapper);
			var nonExistingId = DbContext.Firms.OrderByDescending(f => f.Id).First().Id + 100;
			var query = new GetFirmQuery(nonExistingId);

			// Act / Assert
			await Assert.ThrowsAsync<NotFoundExceptionOverride>(async () =>
			{
				await handler.Handle(query, CancellationToken.None);
			});
		}
	}
}
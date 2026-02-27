using AutoMapper;
using Microsoft.Extensions.Logging;
using Sai.DealAssistant.Application.Common.Exceptions;
using Sai.DealAssistant.Application.Entities.EventNotes.Queries;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Dtos;
using Sai.DealAssistant.Application.Entities.SampleCustomers.Queries;
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
	public class GetDealQuery_Handler_Test : UnitTestBase
	{
		private readonly ReadRepository<AppDbContext,Deal> _dealRepository;
		private readonly IMapper _mapper;

		public GetDealQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_dealRepository = new ReadRepository<AppDbContext, Deal>(DbContext);

			var cfg = new MapperConfiguration(c => c.AddProfile<DealDto.MappingProfile>(), new LoggerFactory());
			_mapper = cfg.CreateMapper();

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				var s = new DealState { State = "New" };
				var t = new DealType { Type = "Standard" };
                var at = new AmountType { Type = "Per Month" };
                db.AddRange(s, t, at);
				db.SaveChanges();

				var deals = Enumerable.Range(1, 3).Select(i => new Deal
				{
					Name = $"Deal {i}",
					Description = $"Desc {i}",
					Industry = i % 2 == 0 ? "Software" : "Finance",
					StateId = s.Id,
					TypeId = t.Id,
					AmountTypeId = at.Id
                }).ToArray();

				db.Deals.AddRange(deals);
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_ReturnsDealDto_WhenDealExists()
		{
			// Arrange
			var handler = new GetDealQuery.Handler(_dealRepository, _mapper);
			var expectedEntity = DbContext.Deals.OrderBy(d => d.Id).First();
			var query = new GetDealQuery(expectedEntity.Id);

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedEntity.Id, result.Id);
			Assert.Equal(expectedEntity.Name, result.Name);
			Assert.Equal(expectedEntity.Description, result.Description);
			Assert.Equal(expectedEntity.Industry, result.Industry);
			Assert.Equal(expectedEntity.TypeId, result.TypeId);
			Assert.Equal(expectedEntity.StateId, result.StateId);
		}

		[Fact]
		public async void Handler_ThrowsNotFoundException_WhenDealDoesNotExist()
		{
			// Arrange
			var handler = new GetDealQuery.Handler(_dealRepository, _mapper);
			var nonExistingId = DbContext.Deals.OrderByDescending(d => d.Id).First().Id + 100;
			var query = new GetDealQuery(nonExistingId);

			// Act / Assert
			await Assert.ThrowsAsync<NotFoundExceptionOverride>(async () =>
			{
				await handler.Handle(query, CancellationToken.None);
			});
		}
	}
}
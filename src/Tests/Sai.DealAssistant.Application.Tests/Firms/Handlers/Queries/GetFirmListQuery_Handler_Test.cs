using AutoMapper;
using Sai.DealAssistant.Application.Entities.Firms.Dtos;
using Sai.DealAssistant.Application.Entities.Firms.Queries;
using Sai.DealAssistant.Common.Enums;
using Sai.DealAssistant.Domain.Entities;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Infrastructure.Repositories.Generic;
using SAI.DealAssistant.TestUtils.Unit;
using System.Linq;
using System.Threading;
using Xunit;

namespace Sai.DealAssistant.Application.Tests.Firms.Handlers.Queries
{
	public class GetFirmListQuery_Handler_Test : UnitTestBase
	{
		private readonly ReadRepository<AppDbContext, Firm> _firmRepository;

		public GetFirmListQuery_Handler_Test()
			: base(seedTestData: false)
		{
			_firmRepository = new ReadRepository<AppDbContext, Firm>(DbContext);

			// Seed test data
			using (var db = CreateNewDbContext())
			{
				var firms = Enumerable.Range(1, 20).Select(i => new Firm
				{
					Name = i % 3 == 0 ? $"Special Firm {i}" : $"Firm {i}",
					Country = i % 2 == 0 ? "Germany" : "USA",
					Description = $"Description {i}"
				}).ToArray();

				db.Firms.AddRange(firms);
				db.SaveChanges();
			}
		}

		[Fact]
		public async void Handler_FiltersByName()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			var nameFragment = "Special Firm";
			int pageSize = 100;
			var query = new GetFirmListQuery { Name = nameFragment, Page = 1, PageSize = pageSize };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Firms
				.Where(f => f.Name.Contains(nameFragment))
				.Select(f => new FirmListItemDto
				{
					Id = f.Id,
					Name = f.Name,
					Country = f.Country,
				})
				.Take(pageSize)
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Count, result.Items.Count);
			Assert.Equivalent(expected.OrderBy(x => x.Id), result.Items.OrderBy(x => x.Id));
		}

		[Fact]
		public async void Handler_ReturnsAll_WhenNoFilter()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			int pageSize = 100;
			var query = new GetFirmListQuery { Page = 1, PageSize = pageSize };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var total = DbContext.Firms.Count();
			Assert.Equal(total, result.TotalItems);
			Assert.Equal(total, result.Items.Count);
		}

		[Fact]
		public async void Handler_SortsByName_Ascending()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			var query = new GetFirmListQuery { SortBy = nameof(Firm.Name), SortDirection = SortDirections.Asc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Firms
				.OrderBy(f => f.Name)
				.Select(f => new FirmListItemDto
				{
					Id = f.Id,
					Name = f.Name,
					Country = f.Country,
				})
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByName_Descending()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			var query = new GetFirmListQuery { SortBy = nameof(Firm.Name), SortDirection = SortDirections.Desc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Firms
				.OrderByDescending(f => f.Name)
				.Select(f => new FirmListItemDto
				{
					Id = f.Id,
					Name = f.Name,
					Country = f.Country,
				})
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByCountry_Ascending()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			var query = new GetFirmListQuery { SortBy = nameof(Firm.Country), SortDirection = SortDirections.Asc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Firms
				.OrderBy(f => f.Country)
				.Select(f => new FirmListItemDto
				{
					Id = f.Id,
					Name = f.Name,
					Country = f.Country,
				})
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_SortsByCountry_Descending()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);
			var query = new GetFirmListQuery { SortBy = nameof(Firm.Country), SortDirection = SortDirections.Desc, Page = 1, PageSize = 100 };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var expected = DbContext.Firms
				.OrderByDescending(f => f.Country)
				.Select(f => new FirmListItemDto
				{
					Id = f.Id,
					Name = f.Name,
					Country = f.Country,
				})
				.ToList();

			Assert.Equal(expected.Count, result.TotalItems);
			Assert.Equal(expected.Select(x => x.Id), result.Items.Select(x => x.Id));
		}

		[Fact]
		public async void Handler_PaginatesResults()
		{
			// Arrange
			var handler = new GetFirmListQuery.Handler(_firmRepository);

			const int pageSize = 5;
			const int page = 2;
			var query = new GetFirmListQuery { Page = page, PageSize = pageSize };

			// Act
			var result = await handler.Handle(query, CancellationToken.None);

			// Assert
			var total = DbContext.Firms.Count();
			Assert.Equal(total, result.TotalItems);
			Assert.Equal(pageSize, result.Items.Count);
		}
	}
}